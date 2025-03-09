using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;

namespace CleanArchitecture.Infrastructure.Persistence.Contexts;

//Unable to create a 'DbContext' of type 'ApplicationDbContext'. The exception 'Cannot resolve scoped service 'CleanArchitecture.Application.Interfaces.IAuthenticatedUserService' from root provider.' was thrown while attempting to create an instance. For the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728
//for above error comment IAuthenticatedUserService on db-migration time

//public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)//comment this line for db migration as scoped service issue
//{
//}

//if any issue in db creation,use update-database using migration then it gets created.
//if still issue then comment the ctor with IAuthenticatedUserService then uncomment for normal usage

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
    private IAuthenticatedUserService authenticatedUser;

    //on migration disable this ctor
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options
   , IAuthenticatedUserService _authenticatedUser) : this(options)
        {
        authenticatedUser = _authenticatedUser;
        }

    public DbSet<Product> Products { get; set; }
    public DbSet<CardType> CardTypes { get; set; }
    public DbSet<Town> Towns { get; set; }

    public DbSet<CardTrash> CardTrashes { get; set; }//on deletion full card details will be stored here

    //  public DbSet<UserTrash> UserTrashes { get; set; }//moved to IdentityDb
    public DbSet<Card> Cards { get; set; }//every new card enters here

    public DbSet<Card_DraftChanges> Card_DraftChanges { get; set; }//after enough approval makes entry to Card_VerifiedEntries

    public DbSet<Card_AdditionalTown> Card_AdditionalTowns { get; set; }

    public DbSet<CardData> CardDataEntries { get; set; }//DetailedDescription, fburl,yt,phon no,timings
    public DbSet<CardDetail> CardDetails { get; set; }//DetailedDescription, (image1,2,3,6)

    public DbSet<CardApproval> CardApprovals { get; set; }//once verified makes entry here
    public DbSet<CardRating> CardRatings { get; set; }
    public DbSet<CardDisplayDate> CardDisplayDates { get; set; }//selected cards display dates

    public DbSet<UserCardLimits> UserCardLimits { get; set; }

    public DbSet<UserDetail> UserDetails { get; set; }

    //this must be excluded otherwise table gets created even if you use exclude or ignore it wont work
    //public DbSet<Product> Products { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
        if (authenticatedUser != null)
            ChangeTracker.ApplyAuditing(authenticatedUser);//comment this line while Add-Migration

        return await base.SaveChangesAsync(cancellationToken);
        }

    protected override void OnModelCreating(ModelBuilder builder)
        {
        //builder.Ignore<Product>();//this is not working to exclude

        //created table names will have default dbset entity names,if something different required or dbset not having direct entity then it can be created by using like
        //builder.Entity<TownVerifiedCard>().ToTable(nameof(this.Card_VerifiedEntries));

        //master datas id identity disabling
        /*
        builder.Entity<UserInfo>().Property(et => et.Id).ValueGeneratedNever();
        builder.Entity<UserInfo>().Property(e => e.Roles).HasConversion(v => JsonExtensions.Serialize(v), v => JsonExtensions.DeSerialize<List<string>>(v));
        builder.Entity<UserInfo>().Property(e => e.CardIds).HasConversion(v => JsonExtensions.Serialize(v), v => JsonExtensions.DeSerialize<List<int>>(v));
        Instead of above below is the clean code*/

        builder.Entity<UserDetail>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            //entity.Property(e => e.Roles).HasConversion(v => JsonExtensions.Serialize(v), v => JsonExtensions.DeSerialize<List<string>>(v));
            //entity.Property(e => e.CardIds).HasConversion(v => JsonExtensions.Serialize(v), v => JsonExtensions.DeSerialize<List<int>>(v));
        });

        builder.Entity<CardType>().Property(et => et.Id).ValueGeneratedNever();
        builder.Entity<Town>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IsDisabled).HasDefaultValue(false);
            entity.Property(e => e.Created).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.CreatedBy).HasDefaultValue(Guid.Empty);
            //entity.Property(et => et.IdTown).ValueGeneratedNever();

            //entity.HasOne(e => e.TownCardDraft)
            //.WithOne(a => a.Towns)
            //.HasForeignKey<Towns>(x => x.IdTown).IsRequired(false);

            //entity.HasOne(e => e.TownCardVerified)
            //.WithOne(a => a.Towns)
            //.HasForeignKey<Towns>(x => x.IdTown).IsRequired(false);

            //below should not be
            //entity.HasOne(t => t.TownCardDraft)
            //    .WithOne(d => d.Towns)
            //    .HasForeignKey<Card_Drafts>(d => d.IdCard); // Assuming Draft Id is foreign key
            //entity.HasOne(t => t.TownCardVerified)
            //    .WithOne(a => a.Towns)
            //    .HasForeignKey<Card_VerifiedEntries>(a => a.IdCard)
            //    .IsRequired(false); // IsVerifiedEntryExists is nullable

            //entity.HasMany(e => e.Card_VerifiedEntries).WithOne(e => e.Towns)
            //     .HasForeignKey(ct => ct.IdCard);
        });

        builder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IdTown).IsUnique(false);
            entity.HasOne(c => c.OwnerDetail).WithMany(u => u.iCards).HasForeignKey(c => c.IdOwner);

            entity.HasOne(e => e.CardData).WithOne(d => d.iCard)
            .HasForeignKey<CardData>(d => d.Id).OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CardDetail).WithOne(d => d.iCard).HasForeignKey<CardDetail>(d => d.Id).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.DraftChanges).WithOne(v => v.iCard).HasForeignKey<Card_DraftChanges>(v => v.Id).OnDelete(DeleteBehavior.Restrict).IsRequired(false); // Mark relationship as optional
        });
        builder.Entity<CardTrash>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(et => et.Id).ValueGeneratedNever();
        });

        builder.Entity<UserCardLimits>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(et => et.Id).ValueGeneratedNever();
        });

        builder.Entity<Card_AdditionalTown>(entity =>
        {
            entity.HasKey(e => new { e.IdCARD, e.IdTown });
            entity.HasOne(tc => tc.Town)
                    .WithMany(t => t.VerifiedCardsAdditional)
                    .HasForeignKey(tc => tc.IdTown)
                    .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(tc => tc.iCard)
                .WithMany(a => a.AdditionalTownsOfVerifiedCard)
                .HasForeignKey(tc => tc.IdCARD)
                .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<Card_DraftChanges>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(et => et.Id).ValueGeneratedNever();
            entity.HasIndex(e => e.IdTown).IsUnique(false);

            //entity.HasOne(a => a.DraftCard)
            //    .WithOne(a => a.VerifiedCard)
            //    .HasForeignKey<Card_Verified>(a => a.Id)
            //    .OnDelete(DeleteBehavior.Cascade);
        });
        //Cascade:When a DraftCard is deleted, the related Card_Verified is also deleted.
        //Resrtict:prevents deletion
        //NoAction taken by efcore instead database triggers integrity

        builder.Entity<CardData>(entity =>
        {
            entity.Property(et => et.Id).ValueGeneratedNever();
            entity.HasOne(d => d.iCard)
              .WithOne(ns => ns.CardData)
              .HasForeignKey<CardData>(ns => ns.Id)
              .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CardDetail>(entity =>
        {
            entity.Property(et => et.Id).ValueGeneratedNever();
            entity.HasOne(d => d.iCard)
               .WithOne(ns => ns.CardDetail)
               .HasForeignKey<CardDetail>(ns => ns.Id)
               .OnDelete(DeleteBehavior.Cascade);

            //entity.HasOne(d => d.Card_VerifiedEntries)//mentioned in Card_VerifiedEntries
            //   .WithOne(ns => ns.Details)
            //   .HasForeignKey<CardDetails>(ns => ns.Id).IsRequired(false);
        });

        builder.Entity<CardApproval>(entity =>
        {
            entity.HasKey(e => e.Id);

            //entity.HasIndex(e => new { e.IdCard, e.IdTown, e.IdCardOfApprover }).IsUnique();
            //entity.HasAlternateKey(e => new { e.IdCard, e.IdTown, e.IdCardOfApprover });
            //above both are not allowed as it makes idtown or idcardapprover makes non nullable but we need it

            entity.ToTable(t => t.HasCheckConstraint(
     "CK_CardApprovals_AtLeastOneTownOrApprover",
     "([IdTown] IS NOT NULL OR [IdCardOfApprover] IS NOT NULL)"));

            entity.Property(ca => ca.IdTown).IsRequired(false); // Make IdTown nullable
            entity.Property(ca => ca.IdCardOfApprover).IsRequired(false); // Make IdTown nullable

            //entity.HasOne(d => d.iCard)
            //      .WithMany(ns => ns.CardApprovals)
            //      .HasForeignKey(ns => ns.IdCard)
            //      //.OnDelete(DeleteBehavior.NoAction);
            //      .OnDelete(DeleteBehavior.Cascade);//must,otherwise error

            //pending for approver show
            // entity.HasOne(c => c.UserDetail).WithMany(u => u.CardsDraft).HasForeignKey(c => c.IdOwner);
        });

        builder.Entity<CardRating>().HasKey(ca => new { ca.IdCARD, ca.UserId });
        builder.Entity<CardRating>(entity =>
        {
            entity.HasOne(d => d.iCard)
                  .WithMany(ns => ns.CardRatings)
                  .HasForeignKey(ns => ns.IdCARD).OnDelete(DeleteBehavior.Cascade); ;
        });

        //All Decimals will have 18,6 Range, currently no decimals so diabling below
        //foreach (var property in builder.Model.GetEntityTypes()
        //.SelectMany(t => t.GetProperties())
        //.Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        //    {
        //    property.SetColumnType("decimal(18,6)");
        //    }

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        this.ConfigureDecimalProperties(builder);
        base.OnModelCreating(builder);
        }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(message => Log.Information(message), LogLevel.Information);
        }
    }

//this is for only applying migrations time and resolving dependency issue simple way
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
    public ApplicationDbContext CreateDbContext(string[] args)
        {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var configuration = new ConfigurationBuilder()//Microsoft.Extensions.Options.ConfigurationExtensions
            .SetBasePath(Directory.GetCurrentDirectory()) //Microsoft.Extensions.Configuration.FileExtensions
            .AddJsonFile("appsettings.Development.json")//Microsoft.Extensions.Configuration.Json
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

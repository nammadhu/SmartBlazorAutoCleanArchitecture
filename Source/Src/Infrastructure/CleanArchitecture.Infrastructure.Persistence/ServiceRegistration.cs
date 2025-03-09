using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CleanArchitecture.Infrastructure.Persistence;

public static class ServiceRegistration
    {
    public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemoryDatabase)
        {
        if (useInMemoryDatabase)
            {
            //use AddDbContextFactory instead of AddDbContext to avoid second operation query issue
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(nameof(ApplicationDbContext)));
            }
        else
            {
            //// Register DbContextOptions
            //var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            //optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            //https://stackoverflow.com/questions/58046008/blazor-a-second-operation-started-on-this-context-before-a-previous-operation-co
            //use AddDbContextFactory instead of AddDbContext to avoid second operation started error query issue
            //services.AddDbContext<ApplicationDbContext>(options =>
            services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnection)),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(3),
                errorNumbersToAdd: null);
        }));

            services.AddScoped<IDbContextFactory<ApplicationDbContext>>(provider =>
            {
                var options = provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
                var authenticatedUserService = provider.GetRequiredService<IAuthenticatedUserService>();
                var logger = provider.GetRequiredService<ILogger<CustomDbContextFactory<ApplicationDbContext>>>();
                return new CustomDbContextFactory<ApplicationDbContext>(options, authenticatedUserService, logger);
            });
            }
        services.AddScoped<DbContextProvider>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.RegisterRepositories();

        return services;
        }

    private static void RegisterRepositories(this IServiceCollection services)
        {
        //services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        var interfaceType = typeof(IGenericRepository<>);
        var interfaces = Assembly.GetAssembly(interfaceType)!.GetTypes()
            .Where(p => p.GetInterface(interfaceType.Name) != null);

        var implementations = Assembly.GetAssembly(typeof(GenericRepository<>))!.GetTypes();

        foreach (var item in interfaces)
            {
            var implementation = Assembly.GetAssembly(typeof(GenericRepository<>)).GetTypes()
                .FirstOrDefault(p => p.GetInterface(item.Name.ToString()) != null);
            services.AddTransient(item, implementation);
            }
        services.AddScoped<IBackgroundJobsRepository, BackgroundJobsRepository>();
        //BackgroundJobsRepository must be scoped otherwise connection closure happens, and not static also which leads to race conditions
        services.AddTransient<ITownRepository, TownRepository>();
       // services.AddTransient<IUserDetailRepository, UserDetailRepository>(); //for azureAdB2c
        services.AddTransient<ICardTypeRepository, CardTypeRepository>();
        services.AddTransient<ICardRepository, Card_Repository>();
        services.AddTransient<ICard_DraftChangesRepository, Card_DraftChangesRepository>();

        services.AddTransient<ICardDataRepository, CardDataRepository>();
        services.AddTransient<ICardDetailRepository, CardDetailRepository>();

        services.AddTransient<ICard_AdditionalTownRepository, Card_AdditionalTownRespository>();

        services.AddTransient<ICardApprovalRepository, CardApprovalRepository>();

        services.AddTransient<ICleanUpRepository, CleanUpRepository>();

        services.AddScoped<IIDGenerator<Town>>(provider =>
   new IDGenerator<Town>(provider.GetService<DbContextProvider>(), nameof(Town.Id)));
        services.AddScoped<IIDGenerator<CardType>>(provider =>
   new IDGenerator<CardType>(provider.GetService<DbContextProvider>(), nameof(CardType.Id)));
        }
    }

﻿// <auto-generated />
using System;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CleanArchitecture.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250309171104_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CleanArchitecture.Domain.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Address")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.PrimitiveCollection<string>("ApprovedPeerCardIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DisLikeCount")
                        .HasColumnType("int");

                    b.Property<int>("IdCardType")
                        .HasColumnType("int");

                    b.Property<Guid>("IdOwner")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("IdTown")
                        .HasColumnType("int");

                    b.Property<string>("Image1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActiveSubscriber")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAdminVerified")
                        .HasColumnType("bit");

                    b.Property<bool>("IsClaimed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDraftExists")
                        .HasColumnType("bit");

                    b.Property<bool>("IsGoogleVerified")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPeerVerified")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("LikeCount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubTitle")
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("IdCardType");

                    b.HasIndex("IdOwner");

                    b.HasIndex("IdTown");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardApproval", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("IdCard")
                        .HasColumnType("int");

                    b.Property<int?>("IdCardOfApprover")
                        .HasColumnType("int");

                    b.Property<int?>("IdTown")
                        .HasColumnType("int");

                    b.Property<bool?>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("IdCard");

                    b.HasIndex("IdCardOfApprover");

                    b.ToTable("CardApprovals", t =>
                        {
                            t.HasCheckConstraint("CK_CardApprovals_AtLeastOneTownOrApprover", "([IdTown] IS NOT NULL OR [IdCardOfApprover] IS NOT NULL)");
                        });
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardData", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndDateToShow")
                        .HasColumnType("datetime2");

                    b.Property<string>("FaceBookUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GoogleCustomerId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GoogleMapAddressUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GoogleProfileUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("GoogleScoreRating")
                        .HasColumnType("real");

                    b.Property<string>("InstagramUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsGoogleVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Latitude")
                        .HasColumnType("float");

                    b.Property<double?>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("MobileNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OtherReferenceUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortNote")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("TwitterUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("YouTubeUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CardDataEntries");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardDetail", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DetailDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExternalImageLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image5")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image6")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsOpenNow")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MoreImages")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Queue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TimingsToday")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TimingsUsual")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("YouTubeVideoLink")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CardDetails");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardDisplayDate", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("CardId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("CardDisplayDates");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardRating", b =>
                {
                    b.Property<int>("IdCARD")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Liked")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("IdCARD", "UserId");

                    b.ToTable("CardRatings");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardTrash", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("CardDataAsJsonString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CardTrashes");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IconClass")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IconColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IconMarkupString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<byte?>("PriorityOrder")
                        .HasColumnType("tinyint");

                    b.Property<byte>("RequiredApprovalCount")
                        .HasColumnType("tinyint");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CardTypes");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Card_AdditionalTown", b =>
                {
                    b.Property<int>("IdCARD")
                        .HasColumnType("int");

                    b.Property<int>("IdTown")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdCARD", "IdTown");

                    b.HasIndex("IdTown");

                    b.ToTable("Card_AdditionalTowns");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Card_DraftChanges", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Address")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("IdCardType")
                        .HasColumnType("int");

                    b.Property<Guid>("IdOwner")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("IdTown")
                        .HasColumnType("int");

                    b.Property<string>("Image1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("OwnerDetailId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SubTitle")
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("IdCardType");

                    b.HasIndex("IdTown");

                    b.HasIndex("OwnerDetailId");

                    b.ToTable("Card_DraftChanges");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BarCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Town", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AlternateNames")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<Guid>("CreatedBy")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValue(new Guid("00000000-0000-0000-0000-000000000000"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DetailDescription")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("District")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExternalImageLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FaceBookUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GoogleMapAddressUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GoogleProfileUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image5")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image6")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InstagramUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDisabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastCardUpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Latitude")
                        .HasColumnType("float");

                    b.Property<double?>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("MobileNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MoreImages")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OtherReferenceUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PinCodePostal")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Population")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TwitterUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrlName1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrlName2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("VoteCount")
                        .HasColumnType("int");

                    b.Property<string>("YouTubeUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("YouTubeVideoLink")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Towns");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.UserCardLimits", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AllowedCardLimits")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalCardCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalCreatedCardCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalDeletedCardCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalDraftCardCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalVerifiedCardCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UserCardLimits");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.UserDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsLikeCommentSubscribed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.PrimitiveCollection<string>("Roles")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserDetail");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Card", b =>
                {
                    b.HasOne("CleanArchitecture.Domain.CardType", "Type")
                        .WithMany()
                        .HasForeignKey("IdCardType")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CleanArchitecture.Domain.UserDetail", "OwnerDetail")
                        .WithMany("iCards")
                        .HasForeignKey("IdOwner")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CleanArchitecture.Domain.Town", "Town")
                        .WithMany("DraftCards")
                        .HasForeignKey("IdTown")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OwnerDetail");

                    b.Navigation("Town");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardApproval", b =>
                {
                    b.HasOne("CleanArchitecture.Domain.Card", "iCard")
                        .WithMany()
                        .HasForeignKey("IdCard")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CleanArchitecture.Domain.Card_DraftChanges", "ApproverCard")
                        .WithMany("CardApprovals")
                        .HasForeignKey("IdCardOfApprover");

                    b.Navigation("ApproverCard");

                    b.Navigation("iCard");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardData", b =>
                {
                    b.HasOne("CleanArchitecture.Domain.Card", "iCard")
                        .WithOne("CardData")
                        .HasForeignKey("CleanArchitecture.Domain.CardData", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("iCard");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardDetail", b =>
                {
                    b.HasOne("CleanArchitecture.Domain.Card", "iCard")
                        .WithOne("CardDetail")
                        .HasForeignKey("CleanArchitecture.Domain.CardDetail", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("iCard");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardDisplayDate", b =>
                {
                    b.HasOne("CleanArchitecture.Domain.Card", null)
                        .WithMany("VerifiedCardDisplayDates")
                        .HasForeignKey("CardId");

                    b.HasOne("CleanArchitecture.Domain.Card_DraftChanges", "VerifiedCard")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VerifiedCard");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.CardRating", b =>
                {
                    b.HasOne("CleanArchitecture.Domain.Card", "iCard")
                        .WithMany("CardRatings")
                        .HasForeignKey("IdCARD")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("iCard");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Card_AdditionalTown", b =>
                {
                    b.HasOne("CleanArchitecture.Domain.Card", "iCard")
                        .WithMany("AdditionalTownsOfVerifiedCard")
                        .HasForeignKey("IdCARD")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CleanArchitecture.Domain.Town", "Town")
                        .WithMany("VerifiedCardsAdditional")
                        .HasForeignKey("IdTown")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Town");

                    b.Navigation("iCard");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Card_DraftChanges", b =>
                {
                    b.HasOne("CleanArchitecture.Domain.Card", "iCard")
                        .WithOne("DraftChanges")
                        .HasForeignKey("CleanArchitecture.Domain.Card_DraftChanges", "Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("CleanArchitecture.Domain.CardType", "Type")
                        .WithMany()
                        .HasForeignKey("IdCardType")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CleanArchitecture.Domain.Town", "Town")
                        .WithMany("VerifiedCards")
                        .HasForeignKey("IdTown")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CleanArchitecture.Domain.UserDetail", "OwnerDetail")
                        .WithMany()
                        .HasForeignKey("OwnerDetailId");

                    b.Navigation("OwnerDetail");

                    b.Navigation("Town");

                    b.Navigation("Type");

                    b.Navigation("iCard");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Card", b =>
                {
                    b.Navigation("AdditionalTownsOfVerifiedCard");

                    b.Navigation("CardData");

                    b.Navigation("CardDetail");

                    b.Navigation("CardRatings");

                    b.Navigation("DraftChanges");

                    b.Navigation("VerifiedCardDisplayDates");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Card_DraftChanges", b =>
                {
                    b.Navigation("CardApprovals");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Town", b =>
                {
                    b.Navigation("DraftCards");

                    b.Navigation("VerifiedCards");

                    b.Navigation("VerifiedCardsAdditional");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.UserDetail", b =>
                {
                    b.Navigation("iCards");
                });
#pragma warning restore 612, 618
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Infrastructure.Persistence.Migrations
    {
    /// <inheritdoc />
    public partial class Init : Migration
        {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
            {
            migrationBuilder.CreateTable(
                name: "CardTrashes",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CardDataAsJsonString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTrashes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardTypes",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconMarkupString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<int>(type: "int", nullable: false),
                    PriorityOrder = table.Column<byte>(type: "tinyint", nullable: true),
                    RequiredApprovalCount = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    BarCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Towns",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoreImages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YouTubeVideoLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalImageLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Population = table.Column<int>(type: "int", nullable: true),
                    VoteCount = table.Column<int>(type: "int", nullable: true),
                    PinCodePostal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternateNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastCardUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleMapAddressUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleProfileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaceBookUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YouTubeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstagramUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitterUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherReferenceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Towns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCardLimits",
                columns: table => new
                    {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalCardCount = table.Column<int>(type: "int", nullable: false),
                    TotalCreatedCardCount = table.Column<int>(type: "int", nullable: false),
                    TotalVerifiedCardCount = table.Column<int>(type: "int", nullable: false),
                    TotalDraftCardCount = table.Column<int>(type: "int", nullable: false),
                    TotalDeletedCardCount = table.Column<int>(type: "int", nullable: false),
                    AllowedCardLimits = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCardLimits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDetail",
                columns: table => new
                    {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsLikeCommentSubscribed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true),
                    IsAdminVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsPeerVerified = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedPeerCardIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsGoogleVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsDraftExists = table.Column<bool>(type: "bit", nullable: false),
                    IsClaimed = table.Column<bool>(type: "bit", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    DisLikeCount = table.Column<int>(type: "int", nullable: false),
                    IsActiveSubscriber = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdCardType = table.Column<int>(type: "int", nullable: false),
                    IdOwner = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTown = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Image1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_CardTypes_IdCardType",
                        column: x => x.IdCardType,
                        principalTable: "CardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cards_Towns_IdTown",
                        column: x => x.IdTown,
                        principalTable: "Towns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cards_UserDetail_IdOwner",
                        column: x => x.IdOwner,
                        principalTable: "UserDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Card_AdditionalTowns",
                columns: table => new
                    {
                    IdTown = table.Column<int>(type: "int", nullable: false),
                    IdCARD = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card_AdditionalTowns", x => new { x.IdCARD, x.IdTown });
                    table.ForeignKey(
                        name: "FK_Card_AdditionalTowns_Cards_IdCARD",
                        column: x => x.IdCARD,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Card_AdditionalTowns_Towns_IdTown",
                        column: x => x.IdTown,
                        principalTable: "Towns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Card_DraftChanges",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdCardType = table.Column<int>(type: "int", nullable: false),
                    IdOwner = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdTown = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Image1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card_DraftChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Card_DraftChanges_CardTypes_IdCardType",
                        column: x => x.IdCardType,
                        principalTable: "CardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Card_DraftChanges_Cards_Id",
                        column: x => x.Id,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Card_DraftChanges_Towns_IdTown",
                        column: x => x.IdTown,
                        principalTable: "Towns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Card_DraftChanges_UserDetail_OwnerDetailId",
                        column: x => x.OwnerDetailId,
                        principalTable: "UserDetail",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CardDataEntries",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false),
                    EndDateToShow = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShortNote = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    GoogleCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsGoogleVerified = table.Column<bool>(type: "bit", nullable: false),
                    GoogleScoreRating = table.Column<float>(type: "real", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleMapAddressUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleProfileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaceBookUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YouTubeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstagramUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitterUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherReferenceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardDataEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardDataEntries_Cards_Id",
                        column: x => x.Id,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardDetails",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IsOpenNow = table.Column<bool>(type: "bit", nullable: true),
                    TimingsToday = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimingsUsual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Queue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoreImages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YouTubeVideoLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalImageLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardDetails_Cards_Id",
                        column: x => x.Id,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardRatings",
                columns: table => new
                    {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCARD = table.Column<int>(type: "int", nullable: false),
                    Liked = table.Column<bool>(type: "bit", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardRatings", x => new { x.IdCARD, x.UserId });
                    table.ForeignKey(
                        name: "FK_CardRatings_Cards_IdCARD",
                        column: x => x.IdCARD,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardApprovals",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCard = table.Column<int>(type: "int", nullable: false),
                    IdCardOfApprover = table.Column<int>(type: "int", nullable: true),
                    IdTown = table.Column<int>(type: "int", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardApprovals", x => x.Id);
                    table.CheckConstraint("CK_CardApprovals_AtLeastOneTownOrApprover", "([IdTown] IS NOT NULL OR [IdCardOfApprover] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_CardApprovals_Card_DraftChanges_IdCardOfApprover",
                        column: x => x.IdCardOfApprover,
                        principalTable: "Card_DraftChanges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CardApprovals_Cards_IdCard",
                        column: x => x.IdCard,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardDisplayDates",
                columns: table => new
                    {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    CardId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardDisplayDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardDisplayDates_Card_DraftChanges_Id",
                        column: x => x.Id,
                        principalTable: "Card_DraftChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardDisplayDates_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Card_AdditionalTowns_IdTown",
                table: "Card_AdditionalTowns",
                column: "IdTown");

            migrationBuilder.CreateIndex(
                name: "IX_Card_DraftChanges_IdCardType",
                table: "Card_DraftChanges",
                column: "IdCardType");

            migrationBuilder.CreateIndex(
                name: "IX_Card_DraftChanges_IdTown",
                table: "Card_DraftChanges",
                column: "IdTown");

            migrationBuilder.CreateIndex(
                name: "IX_Card_DraftChanges_OwnerDetailId",
                table: "Card_DraftChanges",
                column: "OwnerDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CardApprovals_IdCard",
                table: "CardApprovals",
                column: "IdCard");

            migrationBuilder.CreateIndex(
                name: "IX_CardApprovals_IdCardOfApprover",
                table: "CardApprovals",
                column: "IdCardOfApprover");

            migrationBuilder.CreateIndex(
                name: "IX_CardDisplayDates_CardId",
                table: "CardDisplayDates",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_IdCardType",
                table: "Cards",
                column: "IdCardType");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_IdOwner",
                table: "Cards",
                column: "IdOwner");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_IdTown",
                table: "Cards",
                column: "IdTown");
            }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
            {
            migrationBuilder.DropTable(
                name: "Card_AdditionalTowns");

            migrationBuilder.DropTable(
                name: "CardApprovals");

            migrationBuilder.DropTable(
                name: "CardDataEntries");

            migrationBuilder.DropTable(
                name: "CardDetails");

            migrationBuilder.DropTable(
                name: "CardDisplayDates");

            migrationBuilder.DropTable(
                name: "CardRatings");

            migrationBuilder.DropTable(
                name: "CardTrashes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "UserCardLimits");

            migrationBuilder.DropTable(
                name: "Card_DraftChanges");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "CardTypes");

            migrationBuilder.DropTable(
                name: "Towns");

            migrationBuilder.DropTable(
                name: "UserDetail");
            }
        }
    }

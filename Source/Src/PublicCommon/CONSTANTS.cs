using System.Text.Json;
using System.Text.Json.Serialization;

namespace PublicCommon
{
    //cachekey refreshinterval set inside VoteCacheKey class refreshInterval

    public class ApplicationTypes
    {
        public static readonly List<ApplicationType> Apps = [new ApplicationType( Katthe,"Katthe.in"),
            new(MyVote),
            new(MyTown,new List<string>(){"SmartTown.in" }),
            new(MyProducts)
        ];

        public const string Katthe = "Katthe";
        public const string MyVote = "MyVote";
        public const string MyTown = "SmartTown";
        public const string MyProducts = "MyProducts";
    }

    public class ApplicationType
    {
        public ApplicationType(string name)
        {
            Name = name;
        }

        public ApplicationType(string name, string url) : this(name)
        {
            Urls = [url];
        }

        public ApplicationType(string name, List<string> urls) : this(name)
        {
            Urls = urls;
        }

        public byte Index { get; set; }
        public string Name { get; set; }
        public List<string>? Urls { get; set; }
    }

    public enum ApplicationTypeEnum
    {
        Katthe = 0,
        MyVote = 1,
        MyTown = 2,
        MyProducts = 3
    }

    public enum EnvironmentEnum
    {
        Development,
        Test,
        Production
    }

    public static class CONSTANTS
    {
        public const string UrlMain = "https://smarttown.in";
        public static readonly TimeSpan Default_MaxCacheTimeSpan = TimeSpan.FromDays(15);
        public static readonly TimeSpan Default_MinCacheTimeSpan = TimeSpan.FromHours(6);

        public const string TimingsUsualDefault = "9.30am-1pm Lunch 2pm-6.30pm & Sunday Holiday";
        public const string TimingsToday = "9.30am-1pm Lunch 2pm-6.30pm";

        public const string PitchDeckUrl = "https://drive.google.com/file/d/1IW9ixmzJCD92AXF1_JFba6YTebb5OMEz/view?usp=drive_link";
        public const string PitchDeckUrlPptx = "https://docs.google.com/presentation/d/10yJYzb-ywOzXyGn_YxA8XAJqlRXDRDqF/view?usp=sharing&ouid=110847474303853882887&rtpof=true&sd=true";
        public const string PitchVideoUrl = "https://www.youtube.com/embed/fdqAJUIPYjA?si=u-K4pKw9LxCZQdjM";//7 min

        public const string MadhuResumeUrl = "https://drive.google.com/file/d/10q_BmO5TaaG8P-bx3hi1ynASVscUqwWq/view?usp=drive_link";

        public const string VideoUrl = "https://www.youtube.com/embed/n6zm6HCHTOE?si=jDiX-9kgmC28BUQF&autoplay=1&mute=1";
        public const string MadhuVideoUrl = "https://www.youtube.com/embed/J-5BV4pIDAE?si=roda5C9UcaqMmNAE&autoplay=1&mute=1";

        public static readonly JsonSerializerOptions DefaultSerializationJsonOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,//| JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
            //,WriteIndented = true, //this makes payload larger by making indentation
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase//if necessary
        };

        public const string Base64ImagePrefix = "data:image/";//"data:image/png";

        public const string LoginPath = "MicrosoftIdentity/Account/SignIn";//"account/login";
        public const string LogOutPath = "MicrosoftIdentity/Account/SignOut";//"authentication/logout";
        //"?returnUrl=";//"account/login?returnUrl=/CardTypes"

        public const string DefaultImageExtension = "JPG";

        public const int DefaultApprovalRequired = 3;
        public const string ClientAnonymous = "AnonymousClient";
        public const string ClientAuthorized = "AuthClient";

        public const string ClientConfigurations = "ClientConfigurations";
        public const string MyTownApp = "MyTown";
        public const string MyTownAppKey = "mYtOWN";
        public const string MyTownAppKeyAuth = "mYtOWNsECURE";
        public const string AppKeyName = "X-Encrypted-Content";

        public const string Bearer = "Bearer";
        public const string Authorization = "Authorization";
        public const string ApplicationJson = "application/json";

        public const string Email = "Email";
        public const string UserId = "UserId";

        public const string LocalApiIssuedJwtKey = "ApiIssuedJwtKey";
        public const string LocalConfigurationKey = "Configuration";

        public static class EnvironmentConsts
        {
            public const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
            public const string StorageAccountConnectionString = "StorageAccountConnectionString";// nameof(StorageAccountConnectionString);
            public static readonly string Name = Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT) ?? "Development";
            public const string Development = nameof(EnvironmentEnum.Development);
            public const string Production = nameof(EnvironmentEnum.Production);
            public const string Test = nameof(EnvironmentEnum.Test);
            public static readonly bool IsDevelopment = (Name == Development);
            public static readonly bool IsProduction = (Name == Production);
        }

        public static class ROLES
        {
            public const string Role_Admin = "Admin";
            public const string Role_InternalAdmin = "InternalAdmin";
            public const string Role_InternalViewer = "InternalViewer";

            public const string Role_TownAdmin = "TownAdmin";
            public const string Role_TownReviewer = "TownReviewer";

            /// <summary>
            /// once card created then he becomes Creator but then he can transfer
            /// </summary>
            public const string Role_CardCreator = "CardCreator";

            /// <summary>
            /// CardOwner by default when he created, otherwise on transferring of card
            /// </summary>
            public const string Role_CardOwner = "CardOwner";

            /// <summary>
            /// when any card verified he gets this (VerifiedCard table has OwnerId)
            /// </summary>
            public const string Role_CardVerifiedOwner = "CardVerifiedOwner";

            /// <summary>
            /// when someone added as reviewer or reviewed by himself
            /// </summary>
            public const string Role_CardVerifiedReviewer = "CardReviewer";

            //Town main page,option works with Owner role only,if owner then only he can edit,not with creator

            //Any AuthenticatedUser //no separate role required
            //Anonymous //no separate role required

            public const string Role_Blocked = "Blocked";

            public static readonly List<string> AdminWriters = [Role_Admin, Role_InternalAdmin];
            public static readonly List<string> AdminViewers = [Role_Admin, Role_InternalAdmin, Role_InternalViewer, Role_TownAdmin, Role_TownReviewer];

            public static readonly List<string> Approvers = [Role_Admin, Role_InternalAdmin, Role_InternalViewer,
                Role_CardCreator,Role_CardOwner,Role_CardVerifiedOwner,Role_CardVerifiedReviewer,Role_TownAdmin,Role_TownReviewer];

            public static readonly string ApproversAsCsv = $"{Approvers.ToCsv()}";

            public static string TownAdmin(int townId) => $"{Role_TownAdmin}_{townId}";

            public static string TownReviewer(int townId) => $"{Role_TownReviewer}_{townId}";

            public static List<string> TownAdminWriters(int townId = 0)
            {
                if (townId > 0)
                    return AdminWriters.Concat([TownAdmin(townId)]).ToList();
                else
                    return AdminWriters;
            }

            public static List<string> TownAdminReviewers(int townId = 0)
            {
                if (townId > 0)
                    return AdminViewers.Concat([TownReviewer(townId)]).ToList();
                else
                    return AdminViewers;
            }
        }

        //public static class ExternalProviders
        //    {
        //    public const string Google = "Google";
        //    public const string Facebook = "Facebook";
        //    public const string Twitter = "Twitter";
        //    public const string Microsoft = "Microsoft";
        //    }

        //public const string UserIDPData = "UserIDPData";
    }
}
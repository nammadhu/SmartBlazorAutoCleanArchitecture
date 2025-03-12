namespace SHARED
    {
    public class ConstantsTown
        {
        //public const string TownType = "Town";
        //public const int TownTypeId = 1;

        public const string TownMainImagesPrefix = "Town.";

        public const string CardMainImagesPrefix = "Card.";
        public const string CardDetailImagesPrefix = "CardDetail.";

        public const int MaxAllowedTimingsPerDay = 2;

        public const string CardMainImagesPrefix_Keyword = "CI";
        public const string CardDetailImagesPrefix_Keyword = "DI";

        public static readonly CardType CardTypeDefaultGeneralCommon = new CardType(81, "Default General Common Other", "mdi mdi-heart", "#F44336", "&#10084;");

        //new (ConstantsTown.TownTypeId,ConstantsTown.TownType),
        //usage <MudIcon Class="mdi mdi-flash" Style="color: #FF5722; font-size: 36px;" />
        //new (0, "General Common Default Card_Drafts", "mdi-cards-outline", "#4CAF50"), // Green
        // High Priority
        public static readonly List<CardType> cardTypes = [
     new CardType(1, "Priority Important Alert", "mdi mdi-alert-circle", "#FF0000", "&#9888;"), // Warning sign
    new CardType(2, "Premium", "mdi mdi-flash", "#FF5722", "&#9889;"), // High voltage
    new (3, "Event Sport Film Movie Drama", "mdi mdi-calendar-star", "#2196F3", "&#127916;"), // Clapper board
    new (4, "Standard", "mdi mdi-store", "#9C27B0", "&#127978;"), // Convenience store
    new (5, "Doctor Clinic Hospital Pharmacy Health Nursing", "mdi mdi-hospital", "#F44336", "&#127973;"), // Hospital
    new (6, "Laboratory Scanning Diagnostic Blood Testing", "mdi mdi-microscope", "#F44336", "&#128300;"), // Microscope
    new (7, "School College Tuition Education", "mdi mdi-school", "#FFC107", "&#127891;"), // Graduation cap

    // Business Types
    new (11, "New Vehicle Dealer Bike Scooter Car Tractor Auto Tire Tyre", "mdi mdi-bike", "#607D8B", "&#128690;"), // Bicycle
    new (12, "Service Garage Vehicle Bike Car Scooter Tractor BiCycle Auto Repair Wash Puncture", "mdi mdi-tools", "#607D8B", "&#128295;"), // Tools

    new (16, "Hardware", "mdi mdi-hammer-wrench", "#607D8B", "&#128296;"), // Hammer and wrench
    new (17, "Hotel Lodge Restaurant Food", "mdi mdi-silverware-variant", "#795548", "&#127858;"), // Fork and knife
    new (18, "Textiles Tailors Designers Stitch Laundry Dry Clean", "mdi mdi-tshirt-crew", "#3F51B5", "&#128085;"), // Shirt
    new (19, "Beauticians Saloons Hair Cut", "mdi mdi-hair-dryer", "#E91E63", "&#128136;"), // Hair dryer
    new (20, "Electricals Home Appliances Electronics", "mdi mdi-fridge-outline", "#00BCD4", "&#128421;"), // Electric plug
    new (21, "Convention Hall Kalyana Mantapa", "mdi mdi-home-city", "#8BC34A", "&#127968;"), // House
    new (22, "Shops Provision Stores Super Markets", "mdi mdi-cart", "#FF9800", "&#128722;"), // Shopping cart
    new (23, "Gas Agency Petrol Bunks", "mdi mdi-gas-station", "#FFEB3B", "&#9889;"), // High voltage
    new (24, "Bank Govt Offices Government", "mdi mdi-bank", "#673AB7", "&#127974;"), // Bank
    new (25, "Footwear Slipper Shoes", "mdi mdi-shoe-print", "#673AB7", "&#128096;"), // Shoe
    new (26, "House Nurse Doctors Physiotherapy", "mdi mdi-hospital", "#673AB7", "&#127973;"), // Hospital
    new (27, "Pooje Worship Ritual Religion Pooja Pandit Poojari Priest", "mdi mdi-candle", "#673AB7", "&#128367;"), // Candle
    new (28, "Lawyer Court", "mdi mdi-scale-balance", "#673AB7", "&#9878;"), // Scales
    new (29, "Registration Stamp Vendors", "mdi mdi-file-document", "#673AB7", "&#128196;"), // Document
    new (30, "Tourist Packages Bus, Train, Flight Agents", "mdi mdi-airplane", "#673AB7", "&#9992;"), // Airplane
    new (31, "Volunteers Ready To Help Work", "mdi mdi-help-circle", "#673AB7", "&#10067;"), // Question mark
    new (32, "Brokers RTO Vehicle", "mdi mdi-handshake", "#009688", "&#129309;"), // Handshake
    //new (33, "Brokers Marriage", "mdi mdi-handshake", "#009688", "&#129309;"), // Handshake

    new (41, "TV Electronics Repair Service", "mdi mdi-monitor", "#009688", "&#128241;"), // Monitor
    new (42, "Mobile Repair Service", "mdi mdi-cellphone", "#009688", "&#128241;"), // Mobile phone
    new (43, "Computer Laptop CCTV Camera Sales Service", "mdi mdi-laptop", "#009688", "&#128187;"), // Laptop
    new (44, "Gold Jewelry", "mdi mdi-ring", "#FFD700", "&#128142;"), // Ring
    new (45, "Plumber & Carpenter", "mdi mdi-wrench", "#009688", "&#128295;"), // Wrench
    new (46, "House Contractor Builder Home Development Tiles", "mdi mdi-home-currency-usd", "#009688", "&#127968;"), // House
    new (47, "Farmer Milk Chicken Poultry Meat Fish Agriculture", "mdi mdi-cow", "#009688", "&#128004;"), // Cow
    new (48, "BuyOrSale Real Estate Resale", "mdi mdi-home-search", "#009688", "&#127968;"), // House
    new (49, "Pets Bird Dog Aquarium Fish", "mdi mdi-paw", "#009688", "&#128054;"), // Dog

    new (50, "Gift Fancy Stores", "mdi mdi-gift", "#009688", "&#127873;"), // Gift
    new (51, "Photo Video Studio Album", "mdi mdi-camera", "#009688", "&#128248;"), // Camera
    new (52, "Small", "mdi mdi-store", "#9C27B0", "&#127978;"), // Convenience store

    new (61, "Rent My Electronics", "mdi mdi-television", "#009688", "&#128250;"), // Television
    new (62, "Rent My Tools & Equipment", "mdi mdi-toolbox", "#009688", "&#128296;"), // Toolbox
    new (63, "Rent My Books", "mdi mdi-book-open", "#009688", "&#128214;"), // Book
    new (64, "Rent My Sports Equipment", "mdi mdi-basketball", "#009688", "&#127936;"), // Basketball
    new (65, "Rent My General Product Appliances", "mdi mdi-cart", "#009688", "&#128722;"), // Shopping cart
    new (66, "Rent My Furniture", "mdi mdi-sofa", "#009688", "&#128716;"), // Sofa

    CardTypeDefaultGeneralCommon,//new (81, "General Common", "mdi mdi-heart", "#F44336", "&#10084;"), // Heart

    new (90, "Charity", "mdi mdi-heart", "#F44336", "&#10084;"), // Heart
    // Emergency and Issues
    new (91, "Help Emergency (Blood/Any) QUICK", "mdi mdi-alert-circle", "#F44336", "&#9888;"), // Warning sign
    new (92, "Open Issue Problems", "mdi mdi-alert", "#FF5722", "&#9888;"), // Warning sign
    new (93, "Jobs Available Openings", "mdi mdi-briefcase-search", "#3F51B5", "&#128188;"), // Briefcase
    new (94, "Add Resume Job Looking", "mdi mdi-file-document-edit", "#4CAF50", "&#128196;"), // Document
    new (95, "Missing Stolen Theft", "mdi mdi-account-search", "#607D8B", "&#128373;"), // Search
    new (96, "Fraud Cheating Activity", "mdi mdi-alert-octagon", "#FF0000", "&#9940;"), // Stop sign
    new (97, "Illegal Corruption Bribery", "mdi mdi-gavel", "#FF0000", "&#9878;") // Scales
 ];
        }
    }

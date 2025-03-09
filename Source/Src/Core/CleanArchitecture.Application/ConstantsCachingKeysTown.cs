namespace CleanArchitecture.Application;

public class ConstantsCachingServer
    {
    //below are used on server side
    //public const string Config = "Config";
    public const string CacheTownsListKey = "TownsList";

    public const string CacheCardTypesListKey = "CardTypesList";

    public static string CacheCardsOfTownIdKey(int townId) => $"{townId}_TownCards";

    public static string CacheCardsOfTownNameKey(string nameTown) => $"Town_{nameTown}";//ideally should avoid this because leads to confusion and server heavy ,apart from same id already might be

    //so club with id & name together & search

    public static readonly TimeSpan TownsAll_MinCacheTimeSpan = TimeSpan.FromHours(1);
    public static readonly TimeSpan CardTypesAll_MinCacheTimeSpan = TimeSpan.FromHours(12);
    public static readonly TimeSpan Town_MinCacheTimeSpan = TimeSpan.FromHours(1);
    public static readonly TimeSpan Card_MinCacheTimeSpan = TimeSpan.FromMinutes(6);
    public static readonly TimeSpan MyCards_MinCacheTimeSpan = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan ApproverCards_MinCacheTimeSpan = TimeSpan.FromMinutes(30);

    public static readonly TimeSpan UserCardsForAdmin_MinCacheTimeSpan = TimeSpan.FromMinutes(6);
    public static readonly TimeSpan UserCardsForAdmin_MaxCacheTimeSpan = TimeSpan.FromHours(4);
    }

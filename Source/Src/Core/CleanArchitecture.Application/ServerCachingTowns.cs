using BASE;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace CleanArchitecture.Application;

public partial class ServerCachingServiceTowns : ServerCachingBase
    {
    private static ConcurrentDictionary<int, TownDto> _townsDictionary = new ConcurrentDictionary<int, TownDto>();
    public static bool _isCacheLoaded = false;
    private IMemoryCache memoryCache;

    public ServerCachingServiceTowns(IMemoryCache memoryCache) : base(memoryCache)
        {
        this.memoryCache = memoryCache;
        InitializeCache();
        }

    private void InitializeCache()
        {
        if (!_isCacheLoaded)
            {
            if (memoryCache.TryGetValue(ConstantsCachingServer.CacheTownsListKey, out List<TownDto>? initialTowns))
                {
                if (initialTowns != null && initialTowns.Count > 0)
                    {
                    _townsDictionary = new ConcurrentDictionary<int, TownDto>(
                        initialTowns.ToDictionary(town => town.Id, town => town)
                    );
                    _isCacheLoaded = true; // Set to true only when items exist
                    }
                else
                    {
                    _townsDictionary = new ConcurrentDictionary<int, TownDto>();
                    }
                }
            else
                {
                _townsDictionary = new ConcurrentDictionary<int, TownDto>();
                }
            }
        }

    private void SetCache(string key, ConcurrentDictionary<int, TownDto> townsDictionary)
        {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            .SetAbsoluteExpiration(TimeSpan.FromDays(3))
            .SetPriority(CacheItemPriority.High);

        // Serialize and ignore default/null values
        //var townsSerialized = JsonSerializer.Serialize(townsDictionary.Values, _jsonOptions);
        //var townsDeserialized = JsonSerializer.Deserialize<List<TownDto>>(townsSerialized, _jsonOptions);
        //save townsDeserialized
        memoryCache.Set(key, townsDictionary.Values.ToList(), cacheEntryOptions);
        }

    public void AddOrUpdateTownInTowns(TownDto townDto)
        {
        EnsureDictionaryIsPopulated(); // Ensure dictionary is populated before update
        if (townDto != null && townDto.Id > 0)
            {
            townDto.DraftCards = null;//as its unnecessary here
            townDto.VerifiedCards = null;//as its unnecessary here
            if (_townsDictionary != null)
                {
                if (_townsDictionary.ContainsKey(townDto.Id))
                    {
                    _townsDictionary[townDto.Id] = townDto;
                    }
                else _townsDictionary.TryAdd(townDto.Id, townDto);
                SetCache(ConstantsCachingServer.CacheTownsListKey, _townsDictionary);
                }
            }//towns itself not exists in cache,so no need to replace... let it load on next fetch
        }

    public void RemoveTownInTowns(int idTown)
        {
        EnsureDictionaryIsPopulated(); // Ensure dictionary is populated before update
        _townsDictionary.TryRemove(idTown, out _);
        SetCache(ConstantsCachingServer.CacheTownsListKey, _townsDictionary);
        }

    public void UpdateTownsListTownCardUpdatedTimeStamp(int townId, DateTime modifiedTime = default)
        {
        EnsureDictionaryIsPopulated(); // Ensure dictionary is populated before update
        if (_townsDictionary.TryGetValue(townId, out TownDto? town) && town != null)
            {
            town.LastCardUpdateTime = modifiedTime == default ? DateTimeExtension.CurrentTime : modifiedTime;
            _townsDictionary[townId] = town;
            SetCache(ConstantsCachingServer.CacheTownsListKey, _townsDictionary);
            }
        }

    public DateTime? GetTownUpdatedTimeStamp(int townId)
        {
        EnsureDictionaryIsPopulated(); // Ensure dictionary is populated before update
        return _townsDictionary.TryGetValue(townId, out TownDto? town) && town != null ? town.LastModified ?? town.Created : null;
        }

    public bool IsTownLatestThanClient(int townId, DateTime clientTimestamp)
        {
        EnsureDictionaryIsPopulated(); // Ensure dictionary is populated before update
        return _townsDictionary.TryGetValue(townId, out TownDto? town) && town != null &&
            (town.LastModified ?? town.Created) <= clientTimestamp;
        }

    private void EnsureDictionaryIsPopulated()
        {
        if (!_isCacheLoaded)
            {
            InitializeCache();
            }
        }

    public void ManualUpdateTownsDictionary(List<TownDto> towns)
        {
        _townsDictionary = new ConcurrentDictionary<int, TownDto>(
            towns.ToDictionary(town => town.Id, town => town)
        );
        //SetCache(ConstantsCachingServer.CacheTownsListKey, _townsDictionary);
        _isCacheLoaded = true;
        }
    }

using BASE;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Application;

//https://codewithmukesh.com/blog/in-memory-caching-in-aspnet-core/
//Defines a fixed time after which the cache entry will expire, regardless of how often it is accessed. This prevents the cache from serving outdated data indefinitely.
//Sets a time interval during which the cache entry will expire if not accessed.In this example, a 1-HR sliding expiration means that if no one accesses the cache entry within 1 HR, it will be removed.

public class ServerCachingBase(IMemoryCache _memoryCache)
    {
    public static readonly MemoryCacheEntryOptions CacheOptionsDefault = new MemoryCacheEntryOptions()
   .SetAbsoluteExpiration(TimeSpan.FromHours(6))
  .SetSlidingExpiration(TimeSpan.FromHours(1))
  .SetPriority(CacheItemPriority.NeverRemove);

    public static readonly MemoryCacheEntryOptions CacheOptionsAlways = new MemoryCacheEntryOptions()
  .SetAbsoluteExpiration(TimeSpan.MaxValue)
 .SetSlidingExpiration(TimeSpan.MaxValue)
 .SetPriority(CacheItemPriority.NeverRemove);

    public string GetCacheSetTimeKey(string cacheKey) => $"{cacheKey}_TimeStamp";

    public (T? cachedData, DateTime? cacheSetTime) Get<T>(string cacheKey)
        {
        if (_memoryCache.TryGetValue(cacheKey, out T? cachedData))
            {
            if (_memoryCache.TryGetValue(GetCacheSetTimeKey(cacheKey), out DateTime? cacheSetTime))
                {
                return (cachedData, cacheSetTime);
                }
            return (cachedData, DateTimeExtension.CurrentTime);
            }
        return default;
        }

    public (T? cachedData, DateTime? cacheSetTime) Set<T>(string cacheKey, T toSetData, MemoryCacheEntryOptions? options = null)
        {
        if (toSetData != null)
            {
            var result = _memoryCache.Set<T>(cacheKey, toSetData, options);
            var cacheSetTime = DateTimeExtension.CurrentTime;
            _memoryCache.Set<DateTime>(GetCacheSetTimeKey(cacheKey), cacheSetTime, options);
            return (result, cacheSetTime);
            }
        return default;
        }

    public async Task<(T? cachedData, DateTime? cacheSetTime)> GetOrSetAsync<T>(string cacheKey, Func<CancellationToken, Task<T>> getData, MemoryCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
        {
        if (_memoryCache.TryGetValue(cacheKey, out T? cachedData))
            {
            if (_memoryCache.TryGetValue(GetCacheSetTimeKey(cacheKey), out DateTime? cacheSetTime))
                {
                return (cachedData, cacheSetTime);
                }
            return (cachedData, DateTimeExtension.CurrentTime);
            }

        var data = await getData(cancellationToken);
        //_memoryCache.Set(cacheKey, data, options ?? ConstantsCachingKeys.CacheOptionsDefault);
        return Set<T>(cacheKey, data, options ?? CacheOptionsDefault);
        }

    public void Invalidate(string cacheKey)
        {
        _memoryCache.Remove(cacheKey);
        }

    //update had to be separate in each object level
    }

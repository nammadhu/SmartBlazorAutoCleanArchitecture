namespace Shared;

public interface ICachingService
    {
    Task ClearAllAsync(CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task DeleteAsync(string key, CancellationToken cancellationToken = default);

    Task<T?> SetAsync<T>(string key, T? data, CancellationToken cancellationToken = default);
    }

public interface ICachingServiceBaseResult : ICachingService
    {
    Task<T?> AddToListCachedBaseResultDataAsync<T>(string key, T? data, CancellationToken cancellationToken = default);

    Task<(BaseResult<T>? cache, bool cacheStillValid)> GetCachedBaseResultDataAsync<T>(string key, CancellationToken cancellationToken = default);

    Task<(BaseResult<T>? cachedData, bool cacheStillValid)> GetOrSetCachedBaseResultDataAsync<T>(string key, Func<Task<BaseResult<T>?>> fetchData, CancellationToken cancellationToken = default, bool forceFetch = false);

    Task SetBaseResult<T>(string key, BaseResult<T> data, CancellationToken cancellationToken = default);

    Task UpdateBaseResult<T>(string key, T data, CancellationToken cancellationToken = default);

    Task<(BaseResult<T>? cachedData, bool cacheStillValid)> GetOrSetCachedBaseResultDataAsync<T>(string key, HttpClient client, string url, CancellationToken cancellationToken = default, bool forceFetch = false);
    }

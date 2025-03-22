using BASE.Common;

namespace BlazorAuto.Shared.Services;


public interface ICachingServiceClient<T, TKey> where T : class, IAuditableBaseEntity<TKey>
    {
    Task AddOrUpdateAsync(string key, T entity);
    Task AddOrUpdateBulkAsync(List<T> items);
    Task DeleteAsync(string key);
    Task<List<T>> GetAllAsync();
    Task<T?> GetAsync(string key);
    Task InitializeStoreAsync();
    }


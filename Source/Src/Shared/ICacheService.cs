namespace Shared;

public interface ICacheService<T>
    {
    Task<bool> SyncDataAsync(CancellationToken cancellationToken = default);
    Task<List<T>> GetDataAsync(CancellationToken cancellationToken = default);
    }

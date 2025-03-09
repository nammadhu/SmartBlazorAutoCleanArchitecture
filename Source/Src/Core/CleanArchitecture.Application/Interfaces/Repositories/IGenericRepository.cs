namespace CleanArchitecture.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class, IAuditableBaseEntity
    {
    Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetLatestAllAsync(DateTime dateTimeStamp, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    Task<PaginationResponseDto<T>> GetPagedListAsync(int pageNumber, int pageSize, string name, bool getTotalCount, DateTime? minDateTimeToFetch = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

    void Update(T entity);
    void Delete(T entity);
    }

namespace MyTown.Application.Interfaces.Repositories;

public interface ITownRepository : IGenericRepository<Town>
{
    Task<IList<TownDto>> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<PagedResponse<TownDto>> GetPagedListAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken);

    Task<bool> IsNameExistsAsync(string name, CancellationToken cancellationToken);
}
namespace MyTown.Application.Interfaces.Repositories
{
    public interface ICardTypeRepository : IGenericRepository<CardType>
    {
        Task<IList<CardTypeDto>> GetByNameAsync(string name, CancellationToken cancellationToken);

        Task<PagedResponse<CardTypeDto>> GetPagedListAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken);

        Task<bool> IsNameExistsAsync(string name, CancellationToken cancellationToken);
    }
}
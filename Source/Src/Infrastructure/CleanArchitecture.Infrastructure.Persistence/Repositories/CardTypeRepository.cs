using AutoMapper;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
    {
    public class CardTypeRepository(DbContextProvider dbContextProvider, IMapper mapper) : GenericRepository<CardType>(dbContextProvider), ICardTypeRepository
        {
        private const int ResultLimit = 20;
        private readonly DbSet<CardType> db = dbContextProvider.DbContext.Set<CardType>();

        public async Task<PaginationResponseDto<CardTypeDto>> GetPagedListAsync(int pageNumber, int pageSize, string name, bool getTotalCount, DateTime? minDateTimeToFetch = null, CancellationToken cancellationToken = default)
            {
            var query = db.OrderBy(p => p.Created).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                {
                query = query.Where(p => p.Name.Contains(name));
                }

            if (minDateTimeToFetch.HasValue && minDateTimeToFetch.Value > DateTime.MinValue)
                {
                minDateTimeToFetch = minDateTimeToFetch.Value.AddSeconds(1);
                query = query.Where(x => x.Created >= minDateTimeToFetch.Value || (x.LastModified.HasValue && x.LastModified.Value >= minDateTimeToFetch));
                }

            return await Paged(
                    query.Select(p => mapper.Map<CardTypeDto>(p)),
                    pageNumber,
                    pageSize,
                    getTotalCount, cancellationToken);
            }

        public async Task<IList<CardTypeDto>> GetByNameAsync(string name, CancellationToken cancellationToken)
            {
            var query = db.OrderBy(p => p.Created).AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            return await query
                 .OrderByDescending(x => x.Id)
                .Take(ResultLimit).Select(p => mapper.Map<CardTypeDto>(p)).ToListAsync(cancellationToken);
            }

        public async Task<bool> IsNameExistsAsync(string name, CancellationToken cancellationToken)
            {
            return await db.AnyAsync(x => x.Name == name, cancellationToken);
            }
        }
    }

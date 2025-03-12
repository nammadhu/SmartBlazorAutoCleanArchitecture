using BASE;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class CardDetailRepository(DbContextProvider dbContextProvider)
: GenericRepository<CardDetail>(dbContextProvider: dbContextProvider), ICardDetailRepository
    {
    private readonly DbSet<CardDetail> db = dbContextProvider.DbContext.Set<CardDetail>();

    public async Task<int> UpdateOpenClose(int cardId, bool openClose, Guid updatedBy, CancellationToken cancellationToken)
        {
        return await db.Where(x => x.Id == cardId)
          .ExecuteUpdateAsync(x =>
          x.SetProperty(z => z.IsOpenNow, openClose)
          .SetProperty(z => z.LastModifiedBy, updatedBy)
          .SetProperty(z => z.LastModified, DateTimeExtension.CurrentTime)
          , cancellationToken);
        }

    public async Task<int> UpdateTimingsToday(int cardId, string timings, Guid updatedBy, CancellationToken cancellationToken)
        {
        return await db.Where(x => x.Id == cardId)
          .ExecuteUpdateAsync(x => x.SetProperty(z => z.TimingsToday, timings)
          .SetProperty(z => z.LastModifiedBy, updatedBy)
          .SetProperty(z => z.LastModified, DateTimeExtension.CurrentTime), cancellationToken);
        }
    }

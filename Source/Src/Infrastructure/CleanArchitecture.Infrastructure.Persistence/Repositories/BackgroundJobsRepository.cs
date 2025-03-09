using CleanArchitecture.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using MyTown.Application.Interfaces.Repositories;
using MyTown.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class BackgroundJobsRepository(DbContextProvider dbContextProvider) : IBackgroundJobsRepository
//, IMapper mapper)
{
    private const int ResultLimit = 20;
    private readonly DbSet<Town> db = dbContextProvider.DbContext.Set<Town>();

    public async Task<int> MarkTownAsCardsUpdated(int townId, CancellationToken cancellationToken)
    {//this will be used for client side cards update
        try
        {
            return await db.Where(x => x.Id == townId)
                .ExecuteUpdateAsync(x => x.SetProperty(z => z.LastCardUpdateTime, PublicCommon.DateTimeExtension.CurrentTime), cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
    }
}
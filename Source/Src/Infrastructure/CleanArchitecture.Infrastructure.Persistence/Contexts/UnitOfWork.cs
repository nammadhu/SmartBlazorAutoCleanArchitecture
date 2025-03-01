using CleanArchitecture.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Contexts;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken=default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public bool SaveChanges()
    {
        return dbContext.SaveChanges() > 0;
    }
}

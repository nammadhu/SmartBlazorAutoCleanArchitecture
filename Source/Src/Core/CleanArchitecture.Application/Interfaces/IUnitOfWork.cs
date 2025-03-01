using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces;

public interface IUnitOfWork
{
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public interface ICacheService<T>
{
    Task<bool> SyncDataAsync(CancellationToken cancellationToken = default);
    Task<List<T>> GetDataAsync(CancellationToken cancellationToken = default);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Products.DTOs;

namespace SharedResponse;

public interface IOfflineSyncService<T>
{
    Task<bool> SyncDataAsync(CancellationToken cancellationToken = default);
    Task<List<ProductDto>> GetDataAsync(CancellationToken cancellationToken = default);
}

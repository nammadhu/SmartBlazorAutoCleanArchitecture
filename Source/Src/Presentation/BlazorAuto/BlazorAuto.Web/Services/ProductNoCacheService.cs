using SHARED;
using SHARED.DTOs;

namespace BlazorAuto.Web.Services;

public class ProductNoCacheService : ICacheService<ProductDto>
    {
    public async Task<List<ProductDto>> GetDataAsync(CancellationToken cancellationToken = default)
        {
        return null;
        }
    public async Task<bool> SyncDataAsync(CancellationToken cancellationToken = default)
        {
        Console.WriteLine("Am dedicated for webassembly only,sorry I should not have been called");
        return false;
        }
    }

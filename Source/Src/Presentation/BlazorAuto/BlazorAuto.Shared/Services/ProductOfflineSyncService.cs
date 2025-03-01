using CleanArchitecture.Application.Features.Products.Queries.GetPagedListProduct;
using CleanArchitecture.Domain.Products.DTOs;
using Microsoft.EntityFrameworkCore;
using SharedResponse;

namespace BlazorAuto.Shared.Services;

public class ProductOfflineSyncService(ClientCacheDbContext _dbContext, IProduct productService) : IOfflineSyncService<ProductDto>
{
    public async Task<List<ProductDto>> GetDataAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.ToListAsync(cancellationToken);
    }
    public async Task<bool> SyncDataAsync(CancellationToken cancellationToken = default)
    {
        DateTime? latestTimestamp = await _dbContext.Products.AnyAsync(cancellationToken: cancellationToken) ?
                         //.Select(x => new { CreatedDateTime = x.CreatedDateTime, LastModified = x.LastModified ?? DateTime.MinValue })
                         await _dbContext.Products.MaxAsync(x => x.CreatedDateTime, cancellationToken: cancellationToken) : DateTime.MinValue;

        var response = await productService.GetPagedListProductNoCache(new GetPagedListProductQuery()
        { MinDateTimeToFetch = latestTimestamp });
        if (response?.Success == true && response.Data.Count != 0)
        {
            foreach (var item in response.Data)
            {
                var existingItem = await _dbContext.Products.FirstOrDefaultAsync(i => i.Id == item.Id, cancellationToken: cancellationToken);
                if (existingItem == null)
                    await _dbContext.Products.AddAsync(item, cancellationToken);
                else
                {
                    existingItem = item;
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;//modified
        }
        return false;//no modification
        /* avoid api calls here ,instead do separately
        var response = await _httpClient.GetFromJsonAsync<List<ProductDto>>($"api/{nameof(IProduct.GetAllProductList)}");
        if (response != null)
        {
            foreach (var item in response)
            {
                var existingItem = await _dbContext.Products
                    .FirstOrDefaultAsync(i => i.Id == item.Id);
                if (existingItem == null)
                {
                    await _dbContext.Products.AddAsync(item);
                }
                else
                {
                    existingItem.Name = item.Name;
                    existingItem.CreatedDateTime = item.CreatedDateTime;
                }
            }
            await _dbContext.SaveChangesAsync();
        }
        */
    }
}


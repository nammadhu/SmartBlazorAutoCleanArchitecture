using CleanArchitecture.Application.Features.Products.Queries.GetPagedListProduct;
using Microsoft.EntityFrameworkCore;
using SharedResponse;

namespace BlazorAuto.Shared.Services;
public class ProductOfflineSyncService(ClientCacheDbContext _dbContext, IProduct productService)
{
    public async Task SyncDataAsync()
    {
        DateTime? latestTimestamp = await _dbContext.Products.AnyAsync() ?
                         //.Select(x => new { CreatedDateTime = x.CreatedDateTime, LastModified = x.LastModified ?? DateTime.MinValue })
                         await _dbContext.Products.MaxAsync(x => x.CreatedDateTime) : DateTime.MinValue;

        var response = await productService.GetPagedListProductNoCache(new GetPagedListProductQuery()
        { MinDateTimeToFetch = latestTimestamp });
        if (response?.Success == true && response.Data.Count != 0)
        {
            foreach (var item in response.Data)
            {
                var existingItem = await _dbContext.Products.FirstOrDefaultAsync(i => i.Id == item.Id);
                if (existingItem == null)
                    await _dbContext.Products.AddAsync(item);
                else
                {
                    existingItem.Name = item.Name;
                    existingItem.CreatedDateTime = item.CreatedDateTime;
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        /*
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


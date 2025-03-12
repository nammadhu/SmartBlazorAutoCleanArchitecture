using Blazor.IndexedDB;
using SHARED;
using SHARED.DTOs;
using SHARED.Features.Products.Queries;
using SHARED.Interfaces;

namespace BlazorAuto.Web.Client.Services;
//https://github.com/brianly1003/Blazor.IndexedDB
//https://www.syncfusion.com/faq/blazor/general/how-do-i-use-indexeddb-in-blazor-webassembly
public class ProductCacheServiceWasm(IIndexedDbFactory DbFactory, IProduct productService) : ICacheService<ProductDto>
    {
    public async Task<bool> Create(CancellationToken cancellationToken = default)
        {
        using var db = await DbFactory.Create<ClientCacheIndexedDb>();
        var personWithId1 = db.Products.Single(x => x.Id == 1);
        personWithId1.Name = "This is 100% a first name";
        await db.SaveChanges();
        return true;
        }

    public async Task<List<ProductDto>> GetDataAsync(CancellationToken cancellationToken = default)
    => [.. (await DbFactory.Create<ClientCacheIndexedDb>()).Products];


    public async Task<bool> SyncDataAsync(CancellationToken cancellationToken = default)
        {
        using var _dbContext = await DbFactory.Create<ClientCacheIndexedDb>();
        DateTime? latestTimestamp = _dbContext.Products.Count != 0 ?
                         //.Select(x => new { CreatedDateTime = x.CreatedDateTime, LastModified = x.LastModified ?? DateTime.MinValue })
                         _dbContext.Products.Max(x => x.CreatedDateTime) : DateTime.MinValue;

        var response = await productService.GetPagedListProductNoCache(new GetPagedListProductQuery()
            { MinDateTimeToFetch = latestTimestamp });
        if (response?.Success == true && response.Data.Count != 0)
            {
            foreach (var item in response.Data)
                {
                var existingItem = _dbContext.Products.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem == null)
                    _dbContext.Products.Add(item);
                else
                    {
                    existingItem = item;
                    }
                }
            await _dbContext.SaveChanges();
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


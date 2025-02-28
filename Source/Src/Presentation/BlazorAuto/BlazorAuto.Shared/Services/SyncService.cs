using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Features.Products.Queries.GetPagedListProduct;
using CleanArchitecture.Domain.Products.DTOs;
using Microsoft.EntityFrameworkCore;
using SharedResponse;

namespace BlazorAuto.Shared.Services;
public class SyncService( AppDbContext dbContext, IProduct productService)
{
    //IHttpClientFactory httpClientFactory,
    //private readonly HttpClient _httpClient = httpClientFactory.CreateClient(PublicCommon.CONSTANTS.ClientAnonymous);
    private readonly AppDbContext _dbContext = dbContext;
    public async Task SyncDataAsync()
    {
        var response=await productService.GetAllProductList(new GetPagedListProductQuery());
        if (response != null)
        {
            foreach (var item in response.Data)
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


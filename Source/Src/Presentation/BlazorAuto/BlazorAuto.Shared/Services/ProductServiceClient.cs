using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Products.DTOs;

namespace BlazorAuto.Shared.Services;

public class ProductServiceClient
{
    private readonly HttpClient _httpClient;

    public ProductServiceClient(IHttpClientFactory  httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<List<ProductDto>> GetPagedListProduct(int pageNumber, int pageSize)
    {
        var response = await _httpClient.GetAsync($"api/product?pagenumber={pageNumber}&pagesize={pageSize}");
        return await response.Content.ReadFromJsonAsync<List<ProductDto>>();
    }

    public async Task<ProductDto> GetProductById(long id)
    {
        var response = await _httpClient.GetAsync($"api/product/{id}");
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }

    public async Task<long> CreateProduct(ProductDto product)
    {
        var response = await _httpClient.PostAsJsonAsync("api/product", product);
        return await response.Content.ReadFromJsonAsync<long>();
    }

    public async Task UpdateProduct(ProductDto product)
    {
        await _httpClient.PutAsJsonAsync("api/product", product);
    }

    public async Task DeleteProduct(long id)
    {
        await _httpClient.DeleteAsync($"api/product/{id}");
    }
}


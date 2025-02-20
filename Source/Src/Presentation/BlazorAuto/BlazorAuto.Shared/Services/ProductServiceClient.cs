using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Features.Products.Commands.CreateProduct;
using CleanArchitecture.Application.Features.Products.Commands.DeleteProduct;
using CleanArchitecture.Application.Features.Products.Commands.UpdateProduct;
using CleanArchitecture.Application.Features.Products.Queries.GetPagedListProduct;
using CleanArchitecture.Application.Features.Products.Queries.GetProductById;
using CleanArchitecture.Application.Wrappers;
using CleanArchitecture.Domain.Products.DTOs;
using CleanArchitecture.Domain.Products.Entities;
using SharedResponse;

namespace BlazorAuto.Shared.Services;

public class ProductServiceClient(IHttpClientFactory httpClientFactory) : IProduct
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(PublicCommon.CONSTANTS.ClientAnonymous);

  
    public async Task<BaseResult<long>> CreateProduct(CreateProductCommand model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/product", model);
        return await response.Content.ReadFromJsonAsync<long>();
    }

    public async Task<BaseResult> DeleteProduct(DeleteProductCommand model)
    {
        var response = await _httpClient.DeleteAsync($"api/product/{model.Id}");
        return await response.Content.ReadFromJsonAsync<BaseResult>();
    }

    public async Task<PagedResponse<ProductDto>> GetPagedListProduct(GetPagedListProductQuery model)
    {
        var response = await _httpClient.GetAsync($"api/product?pagenumber={model.PageNumber}&pagesize={model.PageSize}");
        return await response.Content.ReadFromJsonAsync<PagedResponse<ProductDto>>();
    }

   
    
    public async Task<BaseResult<ProductDto>> GetProductById(GetProductByIdQuery model)
    {
        var response = await _httpClient.GetAsync($"api/product/{model.Id}");
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }
   
    public async Task<BaseResult> UpdateProduct(UpdateProductCommand model)
    {
        var response = await _httpClient.PutAsJsonAsync("api/product", model);
        return await response.Content.ReadFromJsonAsync<BaseResult>();
    }
}


using BASE;
using SHARED.DTOs;
using SHARED.Features.Products.Commands;
using SHARED.Features.Products.Queries;
using SHARED.Interfaces;
using SHARED.Wrappers;
using System.Net.Http.Json;

namespace BlazorAuto.Shared.Services;

public class ProductServiceClient(IHttpClientFactory httpClientFactory) : IProduct
    {
    //Direct API calls without any client caching here,so can be accessed at any app or wasm
    //if caching required then had to make interface of caching and resolve dependency dynamically
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(CONSTANTS.ClientAnonymous);
    const string apiKey = "api/v1/product/";


    public async Task<BaseResult<long>> CreateProduct(CreateProductCommand model)
        {
        var response = await _httpClient.PostAsJsonAsync($"{apiKey}{nameof(IProduct.CreateProduct)}", model);
        return await response.Content.ReadFromJsonAsync<long>();
        }

    public async Task<BaseResult> DeleteProduct(DeleteProductCommand model)
        {
        var response = await _httpClient.DeleteAsync($"{apiKey}{nameof(IProduct.DeleteProduct)}?{nameof(DeleteProductCommand.Id)}={model.Id}");
        return await response.Content.ReadFromJsonAsync<BaseResult>();
        }

    public async Task<PagedResponse<ProductDto>> GetPagedListProductNoCache(GetPagedListProductQuery model)
     => await ParameterApprend(model, nameof(IProduct.GetPagedListProductNoCache));

    public async Task<PagedResponse<ProductDto>> GetPagedListProduct(GetPagedListProductQuery model)
    => await ParameterApprend(model, nameof(IProduct.GetPagedListProduct));

    async Task<PagedResponse<ProductDto>> ParameterApprend(GetPagedListProductQuery model, string endpoint)
        {
        var response = await _httpClient.GetAsync($"{apiKey}{endpoint}?" +
            $"{nameof(GetPagedListProductQuery.PageNumber)}={model.PageNumber}" +
            $"&{nameof(GetPagedListProductQuery.TotalCount)}={model.TotalCount}" +
        $"&{nameof(GetPagedListProductQuery.Name)}={model.Name}" +
        $"&{nameof(GetPagedListProductQuery.MinDateTimeToFetch)}={model.MinDateTimeToFetch}" +
        $"&{nameof(GetPagedListProductQuery.PageSize)}={model.PageSize}");
        return await response.Content.ReadFromJsonAsync<PagedResponse<ProductDto>>();
        }

    public async Task<BaseResult<ProductDto>> GetProductById(GetProductByIdQuery model)
        {
        var response = await _httpClient.GetAsync($"{apiKey}{nameof(IProduct.GetProductById)}?{nameof(GetProductByIdQuery.Id)}={model.Id}");
        return await response.Content.ReadFromJsonAsync<ProductDto>();
        }

    public async Task<BaseResult> UpdateProduct(UpdateProductCommand model)
        {
        var response = await _httpClient.PutAsJsonAsync($"{apiKey}{nameof(IProduct.UpdateProduct)}", model);
        return await response.Content.ReadFromJsonAsync<BaseResult>();
        }

    }


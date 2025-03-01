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

    public async Task<PagedResponse<ProductDto>> GetPagedListProduct(GetPagedListProductQuery model)
     => await ParameterApprend(model, nameof(IProduct.GetPagedListProduct));

    public async Task<PagedResponse<ProductDto>> GetPagedListProductWithCache(GetPagedListProductQuery model)
    => await ParameterApprend(model, nameof(IProduct.GetPagedListProductWithCache));

    async Task<PagedResponse<ProductDto>> ParameterApprend(GetPagedListProductQuery model, string endpoint)
    {
        var response = await _httpClient.GetAsync($"{apiKey}{endpoint}?" +
            $"{nameof(GetPagedListProductQuery.PageNumber)}={model.PageNumber}" +
            $"&{nameof(GetPagedListProductQuery.GetTotalCount)}={model.GetTotalCount}" +
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


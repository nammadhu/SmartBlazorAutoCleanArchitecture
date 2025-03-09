using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shared.DTOs;
using Shared.Features.Products.Commands;
using Shared.Features.Products.Queries;
using Shared.Wrappers;
using SharedResponse;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArchitecture.WebApi.Controllers.v1;

[ApiVersion("1")]
public class ProductController(IMediator mediator, IMemoryCache cache) : BaseApiController(mediator, cache), IProduct
    {
    private const string CacheKey = "Products";


    [HttpGet]
    public string TestCache()
        {
        cache.TryGetValue("last", out string l1);
        if (l1 == null)
            {
            cache.Set("last", DateTime.Now.ToString());
            cache.TryGetValue("last", out l1);
            return $"am new value:{l1}";
            }
        return $"am from cache:{l1}";
        }
    [HttpGet]//No Cache
    public async Task<PagedResponse<ProductDto>> GetPagedListProductNoCache([FromQuery] GetPagedListProductQuery model)
    => await Mediator.Send(model);

    [HttpGet]
    public async Task<PagedResponse<ProductDto>> GetPagedListProduct([FromQuery] GetPagedListProductQuery model)
        { //TODO change to ALL
        model.Name = string.Empty;
        var cachedList = await GetOrSetCachedListAsync(CacheKey, async () =>
        {
            var result = await Mediator.Send(model);
            return result.Data.ToList();
        }, TimeSpan.FromHours(12));

        // Filter and page the cached list as needed
        var pagedList = cachedList.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
        return new PagedResponse<ProductDto>() { Data = pagedList, PageNumber = model.PageNumber, PageSize = model.PageSize };
        }


    [HttpGet]
    public async Task<BaseResult<ProductDto>> GetProductByIdNoCache([FromQuery] GetProductByIdQuery model)
 => await Mediator.Send(model);

    [HttpGet]
    public async Task<BaseResult<ProductDto>> GetProductById([FromQuery] GetProductByIdQuery model)
        {
        //var cachedList = GetCachedList<ProductDto>(CacheKey);
        //var product = cachedList.FirstOrDefault(p => p.Id == model.Id);
        //instead of doing ,passing Func to baseapi as below
        var product = GetItemFromCachedList<ProductDto>(CacheKey, p => p.Id == model.Id);

        if (product != null)
            {
            return product;
            }

        var result = await Mediator.Send(model);
        var cachedList = GetCachedList<ProductDto>(CacheKey);
        cachedList.Add(result.Data);
        SetOptimizedCachedItem(CacheKey, cachedList);
        return result;
        }

    [HttpPost]//, Authorize]
    public async Task<BaseResult<long>> CreateProduct(CreateProductCommand model) => await Mediator.Send(model);
    //{
    //    var result = await Mediator.Send(model);

    //    // Add the newly created product to the cached list
    //    var cachedList = GetCachedList<ProductDto>(CacheKey);
    //    cachedList.Add(result.Data);
    //    SetOptimizedCachedList(CacheKey, cachedList);

    //    return result;
    //}

    [HttpPut]//, Authorize]
    public async Task<BaseResult> UpdateProduct(UpdateProductCommand model)
        => await Mediator.Send(model);

    [HttpDelete, Authorize]
    public async Task<BaseResult> DeleteProduct([FromQuery] DeleteProductCommand model)
        {
        var result = await Mediator.Send(model);

        // Remove item from cache
        RemoveFromCachedList<ProductDto>(CacheKey, item => item.Id == model.Id);

        return result;
        }

    }

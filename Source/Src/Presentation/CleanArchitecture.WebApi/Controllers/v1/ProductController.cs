using CleanArchitecture.Application.Features.Products.Commands.CreateProduct;
using CleanArchitecture.Application.Features.Products.Commands.DeleteProduct;
using CleanArchitecture.Application.Features.Products.Commands.UpdateProduct;
using CleanArchitecture.Application.Features.Products.Queries.GetPagedListProduct;
using CleanArchitecture.Application.Features.Products.Queries.GetProductById;
using CleanArchitecture.Application.Wrappers;
using CleanArchitecture.Domain.Products.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SharedResponse;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.WebApi.Controllers.v1;

[ApiVersion("1")]
public class ProductController(IMediator mediator, IMemoryCache cache) : BaseApiController(mediator, cache), IProduct
{
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
    [HttpGet]
    public async Task<PagedResponse<ProductDto>> GetPagedListProduct([FromQuery] GetPagedListProductQuery model)
    {
        cache.TryGetValue("last",out string l1);
        cache.Set("last", DateTime.Now.ToString());
     return   await Mediator.Send(model);
    }

    [HttpGet]
    public async Task<BaseResult<ProductDto>> GetProductById([FromQuery] GetProductByIdQuery model)
        => await Mediator.Send(model);

    [HttpPost, Authorize]
    public async Task<BaseResult<long>> CreateProduct(CreateProductCommand model)
        => await Mediator.Send(model);

    [HttpPut, Authorize]
    public async Task<BaseResult> UpdateProduct(UpdateProductCommand model)
        => await Mediator.Send(model);

    [HttpDelete, Authorize]
    public async Task<BaseResult> DeleteProduct([FromQuery] DeleteProductCommand model)
        => await Mediator.Send(model);

}

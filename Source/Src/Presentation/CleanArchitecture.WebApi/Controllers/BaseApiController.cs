using CleanArchitecture.WebApi.Infrastructure.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.WebApi.Controllers;

[ApiController]
[ApiResultFilter]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public abstract class BaseApiController : ControllerBase
{
    private IMediator _mediator;
    private readonly IMemoryCache _cache;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    protected BaseApiController()
    {
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }

    protected BaseApiController(IMediator mediator)
    {
        _mediator ??= mediator;
    }

    protected BaseApiController(IMediator mediator, IMemoryCache cache)
    {
        _mediator ??= mediator;
        _cache = cache;
    }
}

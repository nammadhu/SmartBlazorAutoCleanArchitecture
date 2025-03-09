using BlazorAuto.Web.Services;
using CleanArchitecture.WebApi.Controllers.v1;
using Shared;
using Shared.DTOs;
using SharedResponse;

namespace BlazorAuto.Web;
//This is only for server,no client or maui
public static class DIServerRender
    {
    public static void AddDependencyInjectionServerRender(this IServiceCollection services)
        {
        services.AddScoped<IProduct, ProductController>();
        services.AddScoped<ICacheService<ProductDto>, ProductNoCacheService>();

        }

    }

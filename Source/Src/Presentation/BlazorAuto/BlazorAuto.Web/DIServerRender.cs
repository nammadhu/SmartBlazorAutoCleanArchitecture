using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorAuto.Shared.Services;
using BlazorAuto.Web.Services;
using CleanArchitecture.Domain.Products.DTOs;
using CleanArchitecture.WebApi.Controllers.v1;
using Microsoft.Extensions.DependencyInjection;
using SharedResponse;

namespace BlazorAuto.Web;
//This is only for server,no client or maui
public static class DIServerRender
{
    public static void AddDependencyInjectionServerRender(this IServiceCollection services)
    {
        services.AddScoped<IProduct, ProductController>();
        services.AddScoped<IOfflineSyncService<ProductDto>, ProductOfflineSyncServiceNothing>();
        
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorAuto.Shared.Services;
using CleanArchitecture.Domain.Products.DTOs;
using Microsoft.Extensions.DependencyInjection;
using SharedResponse;

namespace BlazorAuto.Shared;
//This will be shared between client and APP, not server
public static class DIClientRender
{
    public static void AddDependencyInjectionClientRender(this IServiceCollection services)
    {
        services.AddScoped<IProduct, ProductServiceClient>();
      
        services.AddMemoryCache();
        
    }

}

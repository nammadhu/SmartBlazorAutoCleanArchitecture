using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorAuto.Shared.Services;
using CleanArchitecture.WebApi.Controllers.v1;
using Microsoft.Extensions.DependencyInjection;
using SharedResponse;

namespace BlazorAuto.Web;
//This will be shared between client and APP, not server
public static class DIServerRender
{
    public static void ServerRender(this IServiceCollection services)
    {
        services.AddScoped<IProduct, ProductController>();
    }

}

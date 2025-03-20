using BlazorAuto.Web.Services;
using CleanArchitecture.WebApi.Controllers.v1;
using SHARED;
using SHARED.DTOs;
using SHARED.Interfaces;

namespace BlazorAuto.Web;
//This is only for server,no client or maui
public static class DIServerRender
    {
    public static void AddDependencyInjectionServerRender(this IServiceCollection services)
        {
        services.AddScoped<IProduct, ProductController>();
        services.AddScoped<ICardTypeController, CardTypeController>();
        }

    }

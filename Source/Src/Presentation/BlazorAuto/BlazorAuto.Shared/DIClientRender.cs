using BlazorAuto.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using SHARED.Interfaces;

namespace BlazorAuto.Shared;
//This will be shared between client and APP, not server
public static class DIClientRender
    {
    public static void AddDependencyInjectionClientRender(this IServiceCollection services)
        {
        services.AddScoped<IProduct, ProductServiceClient>();
     
        services.AddMemoryCache();


        //for wasm its as 
        //  builder.Services.AddSingleton<SignalRService>(sp => new SignalRService(builder.HostEnvironment.BaseAddress+ "chathub"));

        //for maui its as 
        //services.AddSingleton<SignalRService>(sp => new SignalRService("https://yourserver.com/chathub"));

        }

    }

using BASE;
using Blazor.IndexedDB;
using BlazorAuto.Shared;
using BlazorAuto.Shared.Services;
using BlazorAuto.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using SHARED;
using SHARED.DTOs;
using SHARED.Interfaces;

namespace BlazorAuto.Web.Client;

class Program
    {
    static async Task Main(string[] args)
        {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        // Add device-specific services used by the BlazorAuto.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddDependencyInjectionCommon();//server & client common
        builder.Services.AddDependencyInjectionClientRender();//only client(wasm+MAUI apps)

        builder.Services.AddScoped<ITownCardTypeController, ClientTownCardTypeService>();//only for wasm so here //todo

        builder.Services.AddFluentUIComponents();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthenticationStateDeserialization();

        builder.Services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
        builder.Services.AddScoped<ICacheService<ProductDto>, ProductCacheServiceWasm>();

        //builder.Services.AddHttpClient<ProductServiceClient>("",client =>
        //{
        //    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        //}); //this way also can be used

        builder.Services.AddHttpClient(CONSTANTS.ClientAnonymous, client =>
        {
            client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        });
        builder.Services.AddHttpClient(CONSTANTS.ClientAuthorized, client =>
        {
            client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        });


        builder.Services.AddSingleton<SignalRService>(sp => new SignalRService(builder.HostEnvironment.BaseAddress + "chathub"));

        await builder.Build().RunAsync();
        }
    }

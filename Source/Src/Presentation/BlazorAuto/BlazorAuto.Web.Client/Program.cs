using BASE;
using Blazor.IndexedDB;
using BlazorAuto.Shared;
using BlazorAuto.Shared.Services;
using BlazorAuto.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using SHARED;
using SHARED.DTOs;
using SHARED.Interfaces;
using System.Xml;

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

        //only for wasm so here //todo
        builder.Services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
        //builder.Services.AddSingleton<IndexedDbService<CardTypeDto>>();//wrong
        builder.Services.AddScoped(typeof(IndexedDbService<,>), typeof(IndexedDbService<,>));
        builder.Services.AddScoped(sp =>
        {
            var jsRuntime = sp.GetRequiredService<IJSRuntime>();
            return new IndexedDbService<CardTypeDto, int>(jsRuntime, nameof(CardTypeDto));
            // Factory for creating services with different store names
            //return (Type serviceType) =>
            //{
            //    var storeName = serviceType.Name.Replace("Dto", ""); // Example: Map DTO type to store name
            //    return Activator.CreateInstance(typeof(IndexedDbService<>).MakeGenericType(serviceType), jsRuntime, storeName);
            //};
        });

        //builder.Services.AddScoped(typeof(IndexedDbService<>), sp =>
        //{
        //    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
        //    return (Type serviceType) =>
        //    {
        //        var storeName = serviceType.GetGenericArguments()[0].Name + "Store";
        //        return Activator.CreateInstance(typeof(IndexedDbService<>).MakeGenericType(serviceType.GetGenericArguments()), jsRuntime, storeName);
        //    };
        //});

        builder.Services.AddScoped<ICardTypeController, CientCardTypeService>();
        //builder.Services.AddScoped<ICardController, ClientCardService>();
        //builder.Services.AddScoped<ITownCardsController, ClientTownCardService>();
        //builder.Services.AddScoped<IMyCardsController, ClientMyCardsService>();
        //builder.Services.AddScoped<ITownController, ClientTownService>();

        builder.Services.AddFluentUIComponents();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthenticationStateDeserialization();

       
    
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

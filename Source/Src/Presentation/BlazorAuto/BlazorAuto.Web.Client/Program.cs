using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorAuto.Shared.Services;
using BlazorAuto.Web.Client.Services;
using BlazorAuto.Shared;
using SharedResponse;

namespace BlazorAuto.Web.Client;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        // Add device-specific services used by the BlazorAuto.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();
        
        builder.Services.AddDependencyInjectionCommon();
        builder.Services.AddDependencyInjectionClientRender();
   

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthenticationStateDeserialization();

        //builder.Services.AddHttpClient<ProductServiceClient>("",client =>
        //{
        //    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        //}); //this way also can be used

        builder.Services.AddHttpClient(PublicCommon.CONSTANTS.ClientAnonymous, client =>
        {
            client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        });
        builder.Services.AddHttpClient(PublicCommon.CONSTANTS.ClientAuthorized, client =>
        {
            client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        });


        await builder.Build().RunAsync();
    }
}

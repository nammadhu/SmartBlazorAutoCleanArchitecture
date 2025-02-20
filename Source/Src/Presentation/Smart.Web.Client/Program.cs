using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Smart.Shared.Services;
using Smart.Web.Client.Services;

namespace Smart.Web.Client;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddFluentUIComponents();

        // Add device-specific services used by the Smart.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        //Authentication related
        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthenticationStateDeserialization();


        await builder.Build().RunAsync();
    }
}

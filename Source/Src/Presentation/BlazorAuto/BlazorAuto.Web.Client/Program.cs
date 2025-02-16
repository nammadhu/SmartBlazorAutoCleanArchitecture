using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorAuto.Shared.Services;
using BlazorAuto.Web.Client.Services;

namespace BlazorAuto.Web.Client;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        // Add device-specific services used by the BlazorAuto.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        await builder.Build().RunAsync();
    }
}

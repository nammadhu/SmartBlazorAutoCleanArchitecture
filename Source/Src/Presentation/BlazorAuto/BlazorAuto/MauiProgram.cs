using System.Security.Claims;
using BlazorAuto.Services;
using BlazorAuto.Shared.Services;
using CleanArchitecture.Domain.Products.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedResponse;

namespace BlazorAuto;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add device-specific services used by the BlazorAuto.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddMauiBlazorWebView();


        builder.Services.AddScoped<ICacheService<ProductDto>, ProductCacheServiceMAUI>();
        builder.Services.AddDbContext<ClientCacheSqLiteDbContext>(options =>
            options.UseSqlite("Filename=SmartClientCache.db"));//on client browser memory



        // Add basic authentication and authorization services
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        //builder.Services.AddHttpClient<ProductServiceClient>("",client =>
        //{
        //    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        //}); //this way also can be used

        //builder.Services.AddHttpClient(PublicCommon.CONSTANTS.ClientAnonymous, client =>
        //{
        //    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        //});
        //builder.Services.AddHttpClient(PublicCommon.CONSTANTS.ClientAuthorized, client =>
        //{
        //    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        //});
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser;

    public CustomAuthenticationStateProvider()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public void NotifyUserAuthentication(ClaimsPrincipal user)
    {
        _currentUser = user;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public void NotifyUserLogout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }
}


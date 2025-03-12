using BlazorAuto.Shared;
using BlazorAuto.Shared.Services;
using BlazorAuto.Web.Components;
using BlazorAuto.Web.Components.Account;
using BlazorAuto.Web.Services;
using CleanArchitecture.Application;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.FileManager;
using CleanArchitecture.Infrastructure.FileManager.Contexts;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Identity.Contexts;
using CleanArchitecture.Domain.AspNetIdentity;
using CleanArchitecture.Infrastructure.Identity.Seeds;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using CleanArchitecture.Infrastructure.Persistence.Seeds;
using CleanArchitecture.Infrastructure.Resources;
using CleanArchitecture.WebApi;
using CleanArchitecture.WebApi.Infrastructure.Extensions;
using CleanArchitecture.WebApi.Infrastructure.Middlewares;
using CleanArchitecture.WebApi.Infrastructure.Services;
using CleanArchitecture.Infrastructure.Azure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Serilog;
using BASE;

namespace BlazorAuto.Web;

public class Program
    {
    public static async Task Main(string[] args)
        {
        var builder = WebApplication.CreateBuilder(args);

        var environmentName = builder.Environment.EnvironmentName;
        var applicationName = builder.Environment.ApplicationName;
        var isDevelopment = builder.Environment.IsDevelopment();// Microsoft.Extensions.Hosting.Environments.Development
        var isProduction = builder.Environment.IsProduction();//checks Microsoft.Extensions.Hosting.Environments.Production
        var isStaging = builder.Environment.IsStaging();

        //logger.Warning($"environmentName:{environmentName}");
        //logger.Warning($"applicationName:{applicationName}");
        //logger.Warning($"isDevelopment:{isDevelopment}");
        //logger.Warning($"isProduction:{isProduction}");
        //logger.Warning($"isStaging:{isStaging}");


        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents()
            .AddAuthenticationStateSerialization();
        builder.Services.AddFluentUIComponents();
        // Add device-specific services used by the BlazorAuto.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddDependencyInjectionCommon();
        builder.Services.AddDependencyInjectionServerRender();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        // Add necessary program.cs calls of CleanArchitecture.WebApi
        bool useInMemoryDatabase = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

        builder.Services.AddApplicationLayer();
        builder.Services.AddPersistenceInfrastructure(builder.Configuration, useInMemoryDatabase);
        builder.Services.AddFileManagerInfrastructure(builder.Configuration, useInMemoryDatabase);
        builder.Services.AddIdentityInfrastructure(builder.Configuration, useInMemoryDatabase);

        builder.Services.AddSingleton<AppConfigurations>();
        AppConfigurations config = new();
        config.Initialize(builder.Configuration, environmentName: environmentName, isDevelopment);
        builder.Services.AddSingleton(config);
        builder.Services.AddAzureInfrastructure(builder.Configuration, useInMemoryDatabase);


        builder.Services.AddResourcesInfrastructure();
        builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
        builder.Services.AddControllers();

        builder.Services.AddMemoryCache();  // Add MemoryCache
        builder.Services.AddResponseCaching(); // Add ResponseCaching

        builder.Services.AddVersioning();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddSwaggerWithVersioning();
        builder.Services.AddAnyCors();
        builder.Services.AddCustomLocalization(builder.Configuration);
        builder.Services.AddHealthChecks();

        builder.Services.AddSignalR();

        builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
            {
            var services = scope.ServiceProvider;

            if (!useInMemoryDatabase)
                {
                await services.GetRequiredService<IdentityContext>().Database.MigrateAsync();
                await services.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
                await services.GetRequiredService<FileManagerDbContext>().Database.MigrateAsync();
                }

            // Seed Data
            await DefaultRoles.SeedAsync(services.GetRequiredService<RoleManager<ApplicationRole>>());
            await DefaultBasicUser.SeedAsync(services.GetRequiredService<UserManager<ApplicationUser>>());
            await DefaultData.SeedAsync(services.GetRequiredService<ApplicationDbContext>());
            await Seed.Seeding(app);
            }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
            {
            app.UseWebAssemblyDebugging();
            app.UseMigrationsEndPoint();
            }
        else
            {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(
                typeof(Routes).Assembly,
                typeof(Client._Imports).Assembly);

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        app.UseCustomLocalization();
        app.UseAnyCors();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.UseResponseCaching();// Add ResponseCaching middleware

        app.UseSwaggerWithVersioning();
        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseHealthChecks("/health");
        app.MapControllers();
        app.MapHub<ChatHub>("/chathub");
        app.UseSerilogRequestLogging();

        app.Run();
        }
    }


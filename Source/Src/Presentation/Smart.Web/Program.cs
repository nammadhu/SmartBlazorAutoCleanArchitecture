using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

using Smart.Web.Components;
using Smart.Shared.Services;
using Smart.Web.Services;
using Smart.Web.Components.Account;


using Serilog;
using CleanArchitecture.Application;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.FileManager;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Resources;
using CleanArchitecture.WebApi.Infrastructure.Extensions;
using CleanArchitecture.WebApi.Infrastructure.Middlewares;
using CleanArchitecture.WebApi.Infrastructure.Services;
using CleanArchitecture.Infrastructure.FileManager.Contexts;
using CleanArchitecture.Infrastructure.Identity.Seeds;
using CleanArchitecture.Infrastructure.Identity.Contexts;
using CleanArchitecture.Infrastructure.Persistence.Seeds;
using CleanArchitecture.Infrastructure.Identity.Models;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace Smart;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents()
              .AddAuthenticationStateSerialization();
        builder.Services.AddFluentUIComponents();

        // Add device-specific services used by the Smart.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

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
        builder.Services.AddResourcesInfrastructure();
        builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
        builder.Services.AddControllers();
        builder.Services.AddVersioning();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddSwaggerWithVersioning();
        builder.Services.AddAnyCors();
        builder.Services.AddCustomLocalization(builder.Configuration);
        builder.Services.AddHealthChecks();
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
                typeof(Shared.Routes).Assembly,
                typeof(Web.Client._Imports).Assembly);

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        app.UseCustomLocalization();
        app.UseAnyCors();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.UseSwaggerWithVersioning();
        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseHealthChecks("/health");
        app.MapControllers();
        app.UseSerilogRequestLogging();

        app.Run();
    }
}


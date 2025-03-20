using CleanArchitecture.Domain.AspNetIdentity;
using CleanArchitecture.Infrastructure.Azure;
using CleanArchitecture.Infrastructure.FileManager;
using CleanArchitecture.Infrastructure.FileManager.Contexts;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Identity.Contexts;
using CleanArchitecture.Infrastructure.Identity.Seeds;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using CleanArchitecture.Infrastructure.Persistence.Seeds;
using CleanArchitecture.Infrastructure.Resources;
using CleanArchitecture.WebApi;
using CleanArchitecture.WebApi.Controllers.v1;
using CleanArchitecture.WebApi.Infrastructure.Extensions;
using CleanArchitecture.WebApi.Infrastructure.Middlewares;
using CleanArchitecture.WebApi.Infrastructure.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


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

builder.Services.AddScoped<ICardTypeController, CardTypeController>();


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

    //Seed Data
    await DefaultRoles.SeedAsync(services.GetRequiredService<RoleManager<ApplicationRole>>());
    await DefaultBasicUser.SeedAsync(services.GetRequiredService<UserManager<ApplicationUser>>());
    await DefaultData.SeedAsync(services.GetRequiredService<ApplicationDbContext>());
    await Seed.Seeding(app);
    }

app.UseCustomLocalization();
app.UseAnyCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerWithVersioning();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHealthChecks("/health");

app.UseResponseCaching();// Add ResponseCaching middleware
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.UseSerilogRequestLogging();

app.Run();

public partial class Program
    {
    }

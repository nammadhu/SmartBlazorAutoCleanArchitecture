using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleanArchitecture.Application;

public static class ServiceRegistration
    {
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
        services.AddAutoMapper(config =>
        { config.AddMaps(Assembly.GetExecutingAssembly(), typeof(CardTypeDto).Assembly); });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));//https://github.com/jbogard/MediatR

        //or like below also ok
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly));

        services.AddSingleton<ServerCachingBase>();
        services.AddSingleton<ServerCachingCardTypes>();
        services.AddSingleton<ServerCachingServiceTowns>();
        services.AddSingleton<ServerCachingTownCards>();

        return services;
        }
    }

using CleanArchitecture.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Identity_Azure_AD_B2C;

public static class ServiceRegistration
{
    public static IServiceCollection AddIdentity_Azure_AD_B2C(this IServiceCollection services)
    {
        services.AddScoped<GraphService>();
        services.AddTransient<IIdentityRepository, Identity_Azure_AD_B2CRepository>();

        return services;
    }
}
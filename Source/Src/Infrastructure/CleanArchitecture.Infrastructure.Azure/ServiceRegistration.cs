using CleanArchitecture.Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Azure;

public static class ServiceRegistration
    {
    public static IServiceCollection AddAzureInfrastructure(this IServiceCollection services,
        IConfiguration configuration, bool useInMemoryDatabase)
        {
        //todo need to add Azure connection string setting and all
        services.AddTransient<IAzImageStorage, AzImageStorage>();

        return services;
        }
    }

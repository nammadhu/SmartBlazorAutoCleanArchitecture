using Microsoft.Extensions.DependencyInjection;

namespace BlazorAuto.Shared;
//This will be Common To All
public static class DICommon
    {
    public static IServiceCollection AddDependencyInjectionCommon(this IServiceCollection services)
        {

        return services;
        }
    }

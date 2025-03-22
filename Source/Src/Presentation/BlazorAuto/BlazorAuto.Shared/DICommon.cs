using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorAuto.Shared;
//This will be Common To All
public static class DICommon
    {
    public static IServiceCollection AddDependencyInjectionCommon(this IServiceCollection services)
        {
        services.AddFluentUIComponents();
        services.AddSingleton<DialogService>();

        return services;
        }
    }

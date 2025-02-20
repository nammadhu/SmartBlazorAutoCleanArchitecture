using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorAuto.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using SharedResponse;

namespace BlazorAuto.Shared;
//This will be Common To All
public static class DICommon
{
    public static IServiceCollection AddDependencyInjectionCommon(this IServiceCollection services)
    {

        return services;
    }
}

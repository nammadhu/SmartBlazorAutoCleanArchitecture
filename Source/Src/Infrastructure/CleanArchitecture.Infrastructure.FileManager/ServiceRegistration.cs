using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.FileManager.Contexts;
using CleanArchitecture.Infrastructure.FileManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CleanArchitecture.Infrastructure.FileManager;

public static class ServiceRegistration
    {
    public static IServiceCollection AddFileManagerInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemoryDatabase)
        {
        if (useInMemoryDatabase)
            {
            //use AddDbContextFactory instead of AddDbContext to avoid second operation query issue
            services.AddDbContextFactory<FileManagerDbContext>(options =>
                options.UseInMemoryDatabase(nameof(FileManagerDbContext)));
            }
        else
            {
            //use AddDbContextFactory instead of AddDbContext to avoid second operation query issue
            services.AddDbContextFactory<FileManagerDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("FileManagerConnection")
                , sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(3),
                        errorNumbersToAdd: null);
                }));
            }

        services.AddScoped<IFileManagerService, FileManagerService>();

        return services;
        }
    }

using CleanArchitecture.Domain.Products.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuto.Services;


public class ClientCacheSqLiteDbContext : DbContext
{
    public DbSet<ProductDto> Products { get; set; }

    public ClientCacheSqLiteDbContext(DbContextOptions<ClientCacheSqLiteDbContext> options) : base(options)
    {
        if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        }
        else
        {
            Database.EnsureCreated();
        }
    }
}



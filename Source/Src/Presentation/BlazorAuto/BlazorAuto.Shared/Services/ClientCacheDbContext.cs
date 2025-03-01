using CleanArchitecture.Domain.Products.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuto.Shared.Services;


public class ClientCacheDbContext : DbContext
{
    public DbSet<ProductDto> Products { get; set; }

    public ClientCacheDbContext(DbContextOptions<ClientCacheDbContext> options) : base(options)
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



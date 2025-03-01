using CleanArchitecture.Domain.Products.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuto.Shared.Services;


public class ClientCacheDbContext : DbContext
{
    public DbSet<ProductDto> Products { get; set; }

    public ClientCacheDbContext(DbContextOptions<ClientCacheDbContext> options) : base(options)
    {
        Database.EnsureCreated(); // Ensure the database and tables are created
    }
}



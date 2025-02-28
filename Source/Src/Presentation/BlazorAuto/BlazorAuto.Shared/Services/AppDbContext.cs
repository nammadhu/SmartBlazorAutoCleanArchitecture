using CleanArchitecture.Domain.Products.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuto.Shared.Services;


public class AppDbContext : DbContext
{
    public DbSet<ProductDto> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated(); // Ensure the database and tables are created
    }
}



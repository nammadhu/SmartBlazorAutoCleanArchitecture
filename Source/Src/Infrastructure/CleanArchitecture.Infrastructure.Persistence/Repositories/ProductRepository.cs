using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Products.DTOs;
using CleanArchitecture.Domain.Products.Entities;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext dbContext) : GenericRepository<Product>(dbContext), IProductRepository
{
    public async Task<PaginationResponseDto<ProductDto>> GetPagedListAsync(int pageNumber, int pageSize, string name, bool getTotalCount, DateTime? minDateTimeToFetch = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Products.OrderBy(p => p.Created).AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        if (minDateTimeToFetch.HasValue && minDateTimeToFetch.Value > DateTime.MinValue)
        {
            minDateTimeToFetch = minDateTimeToFetch.Value.AddSeconds(1);
            query = query.Where(x => x.Created >= minDateTimeToFetch.Value || (x.LastModified.HasValue && x.LastModified.Value >= minDateTimeToFetch));
        }

        return await Paged(
                query.Select(p => new ProductDto(p)),
                pageNumber,
                pageSize,
                getTotalCount, cancellationToken);

    }
}

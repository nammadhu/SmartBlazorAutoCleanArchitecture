using CleanArchitecture.Domain.Products.DTOs;
using CleanArchitecture.Domain.Products.Entities;
using SharedResponse.Wrappers;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<PaginationResponseDto<ProductDto>> GetPagedListAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken);
}
using CleanArchitecture.Domain;
using Shared.DTOs;
using Shared.Wrappers;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    //Task<PaginationResponseDto<ProductDto>> GetPagedListAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken);
  
}

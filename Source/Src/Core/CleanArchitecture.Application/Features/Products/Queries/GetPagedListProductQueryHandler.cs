using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Products.DTOs;
using MediatR;
using MyTown.SharedModels.Features.Products.Queries;
using SharedResponse.Wrappers;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Products.Queries;

public class GetPagedListProductQueryHandler(IProductRepository productRepository) : IRequestHandler<GetPagedListProductQuery, PagedResponse<ProductDto>>
{
    public async Task<PagedResponse<ProductDto>> Handle(GetPagedListProductQuery request, CancellationToken cancellationToken)
    {
        return await productRepository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name, cancellationToken);
    }
}

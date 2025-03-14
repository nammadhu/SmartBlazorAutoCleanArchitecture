using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Wrappers;
using CleanArchitecture.Domain.Products.DTOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Products.Queries.GetPagedListProduct;

public class GetPagedListProductQueryHandler(IProductRepository productRepository) : IRequestHandler<GetPagedListProductQuery, PagedResponse<ProductDto>>
{
    public async Task<PagedResponse<ProductDto>> Handle(GetPagedListProductQuery request, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name,request.GetTotalCount, request.MinDateTimeToFetch, cancellationToken: cancellationToken);
    }
}

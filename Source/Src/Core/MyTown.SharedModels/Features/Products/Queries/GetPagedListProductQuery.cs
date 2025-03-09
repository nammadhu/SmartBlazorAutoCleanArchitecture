using CleanArchitecture.Domain.Products.DTOs;
using MediatR;
using SharedResponse.Wrappers;

namespace MyTown.SharedModels.Features.Products.Queries;

public class GetPagedListProductQuery : PaginationRequestParameter, IRequest<PagedResponse<ProductDto>>
{
    public string Name { get; set; }
}

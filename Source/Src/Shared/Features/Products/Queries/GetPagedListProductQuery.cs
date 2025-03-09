using Shared.DTOs;
using Shared.Parameters;

namespace Shared.Features.Products.Queries;

public class GetPagedListProductQuery : PaginationRequestParameter, IRequest<PagedResponse<ProductDto>>
    {
    public string Name { get; set; }
    }

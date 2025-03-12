using SHARED.DTOs;
using SHARED.Parameters;

namespace SHARED.Features.Products.Queries;

public class GetPagedListProductQuery : PaginationRequestParameter, IRequest<PagedResponse<ProductDto>>
    {
    public string Name { get; set; }
    }

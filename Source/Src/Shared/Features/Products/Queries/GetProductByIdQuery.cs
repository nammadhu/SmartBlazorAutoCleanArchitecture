using Shared.DTOs;

namespace Shared.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<BaseResult<ProductDto>>
    {
    public long Id { get; set; }
    }

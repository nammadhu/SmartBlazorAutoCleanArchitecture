using SHARED.DTOs;

namespace SHARED.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<BaseResult<ProductDto>>
    {
    public long Id { get; set; }
    }

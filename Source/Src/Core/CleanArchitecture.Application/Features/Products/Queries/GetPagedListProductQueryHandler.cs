namespace CleanArchitecture.Application.Features.Products.Queries;

public class GetPagedListProductQueryHandler(IProductRepository productRepository, IMapper mapper) : IRequestHandler<GetPagedListProductQuery, PagedResponse<ProductDto>>
    {
    public async Task<PagedResponse<ProductDto>> Handle(GetPagedListProductQuery request, CancellationToken cancellationToken)
        {
        var result = await productRepository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name, false, null, cancellationToken);

        return new PaginationResponseDto<ProductDto>(result.Data.Select(x => mapper.Map<ProductDto>(x)).ToList(), result.Count, request.PageNumber, request.PageSize);
        }
    }

namespace CleanArchitecture.Application.Features.Products.Queries;

public class GetProductByIdQueryHandler(IProductRepository productRepository, ITranslator translator) : IRequestHandler<GetProductByIdQuery, BaseResult<ProductDto>>
    {
    public async Task<BaseResult<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
            {
            return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.Id)), nameof(request.Id));
            }

        return new ProductDto(product);
        }
    }

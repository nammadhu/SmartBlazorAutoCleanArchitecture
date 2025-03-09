using Shared.DTOs;
using Shared.Features.Products.Commands;
using Shared.Features.Products.Queries;

namespace SharedResponse;

public interface IProduct
    {
    Task<BaseResult<long>> CreateProduct(CreateProductCommand model);
    Task<BaseResult> DeleteProduct(DeleteProductCommand model);
    Task<PagedResponse<ProductDto>> GetPagedListProductNoCache(GetPagedListProductQuery model);

    Task<PagedResponse<ProductDto>> GetPagedListProduct(GetPagedListProductQuery model);
    Task<BaseResult<ProductDto>> GetProductById(GetProductByIdQuery model);
    Task<BaseResult> UpdateProduct(UpdateProductCommand model);
    }

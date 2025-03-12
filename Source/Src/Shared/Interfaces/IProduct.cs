using SHARED.DTOs;
using SHARED.Features.Products.Commands;
using SHARED.Features.Products.Queries;

namespace SHARED.Interfaces;

public interface IProduct
    {
    Task<BaseResult<long>> CreateProduct(CreateProductCommand model);
    Task<BaseResult> DeleteProduct(DeleteProductCommand model);
    Task<PagedResponse<ProductDto>> GetPagedListProductNoCache(GetPagedListProductQuery model);

    Task<PagedResponse<ProductDto>> GetPagedListProduct(GetPagedListProductQuery model);
    Task<BaseResult<ProductDto>> GetProductById(GetProductByIdQuery model);
    Task<BaseResult> UpdateProduct(UpdateProductCommand model);
    }

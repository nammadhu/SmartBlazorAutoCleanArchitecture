using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Features.Products.Commands.CreateProduct;
using CleanArchitecture.Application.Features.Products.Commands.DeleteProduct;
using CleanArchitecture.Application.Features.Products.Commands.UpdateProduct;
using CleanArchitecture.Application.Features.Products.Queries.GetPagedListProduct;
using CleanArchitecture.Application.Features.Products.Queries.GetProductById;
using CleanArchitecture.Application.Wrappers;
using CleanArchitecture.Domain.Products.DTOs;

namespace SharedResponse;

public interface IProduct
{
    Task<BaseResult<long>> CreateProduct(CreateProductCommand model);
    Task<BaseResult> DeleteProduct(DeleteProductCommand model);
    Task<PagedResponse<ProductDto>> GetPagedListProduct(GetPagedListProductQuery model);
    Task<PagedResponse<ProductDto>> GetAllProductList(GetPagedListProductQuery model);
    Task<BaseResult<ProductDto>> GetProductById(GetProductByIdQuery model);
    Task<BaseResult> UpdateProduct(UpdateProductCommand model);
}

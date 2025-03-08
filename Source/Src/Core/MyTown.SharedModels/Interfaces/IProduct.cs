using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Products.DTOs;
using MediatR;
using MyTown.SharedModels.Features.Products.Commands;
using MyTown.SharedModels.Features.Products.Queries;

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

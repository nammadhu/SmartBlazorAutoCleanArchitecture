using CleanArchitecture.Domain.Products.DTOs;
using MediatR;

namespace MyTown.SharedModels.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<BaseResult<ProductDto>>
{
    public long Id { get; set; }
}
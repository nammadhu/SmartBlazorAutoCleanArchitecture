using System;
using CleanArchitecture.Application.Parameters;
using CleanArchitecture.Application.Wrappers;
using CleanArchitecture.Domain.Products.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Products.Queries.GetPagedListProduct;

public class GetPagedListProductQuery : PaginationRequestParameter, IRequest<PagedResponse<ProductDto>>
{
    public string Name { get; set; }

    public bool GetTotalCount { get; set; }

    public DateTime? MinDateTimeToFetch { get; set; } //= DateTime.Now;
}

using Shared.DTOs;
using Shared.Parameters;

namespace Shared.Features.CardTypes.Queries;

public class GetCardTypesPagedListQuery : PaginationRequestParameter, IRequest<PagedResponse<CardTypeDto>>
    {
    public bool All { get; set; }
    public string? Name { get; set; }
    }

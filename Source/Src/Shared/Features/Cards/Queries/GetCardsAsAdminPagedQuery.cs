using Shared.DTOs;
using Shared.Parameters;

namespace Shared.Features.Cards.Queries;

public class GetCardsAsAdminPagedQuery : PaginationRequestParameter, IRequest<PagedResponse<CardDto>>
    {//for admin only
    public bool All { get; set; }
    public string? Name { get; set; }
    public int IdTown { get; set; }

    public bool? IsVerified { get; set; }//so fetches from verified table result
    public bool? IsDraft { get; set; }//so fetches from verified table result
    }

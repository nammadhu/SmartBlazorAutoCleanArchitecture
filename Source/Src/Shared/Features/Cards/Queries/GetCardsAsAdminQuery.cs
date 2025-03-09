using Shared.DTOs;

namespace Shared.Features.Cards.Queries;

public class GetCardsAsAdminQuery : IRequest<BaseResult<List<iCardDto>>>
    {//for admin only
    public bool All { get; set; }
    public string? Name { get; set; }
    public int IdTown { get; set; }

    public bool? IsVerified { get; set; }//so fetches from verified table result
    public bool? IsDraft { get; set; }//so fetches from verified table result
    }

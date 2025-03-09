using Shared.DTOs;

namespace Shared.Features.CardTypes.Queries;

public class GetCardTypeByIdQuery : IRequest<BaseResult<CardTypeDto>>
    {
    public int IdCardType { get; set; }
    }

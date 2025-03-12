using SHARED.DTOs;

namespace SHARED.Features.CardTypes.Queries;

public class GetCardTypeByIdQuery : IRequest<BaseResult<CardTypeDto>>
    {
    public int IdCardType { get; set; }
    }

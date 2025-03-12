using SHARED.DTOs;

namespace SHARED.Features.CardTypes.Queries;

public class GetCardTypesAllQuery : IRequest<BaseResult<List<CardTypeDto>>>
    {
    public DateTime? LastCacheTime { get; set; }
    //public bool All { get; set; }
    //public string? Name { get; set; }
    }

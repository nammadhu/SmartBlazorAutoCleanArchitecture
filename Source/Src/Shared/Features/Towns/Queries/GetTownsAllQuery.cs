using SHARED.DTOs;

namespace SHARED.Features.Towns.Queries;

public class GetTownsAllQuery : IRequest<BaseResult<List<TownDto>>>
    {
    public DateTime? LastCacheTime { get; set; }
    //public bool All { get; set; }
    //public string? Name { get; set; }
    }

namespace CleanArchitecture.Application.Features.Towns.Queries;

public class GetTownsAllQueryHandler(ITownRepository townRepo, IMapper mapper, ServerCachingServiceTowns _cachingService) : IRequestHandler<GetTownsAllQuery, BaseResult<List<TownDto>>>
    {
    public async Task<BaseResult<List<TownDto>>> Handle(GetTownsAllQuery request, CancellationToken cancellationToken)
        {
        (List<TownDto>? townsAll, DateTime? cacheSetTime) = await _cachingService.GetOrSetAsync<List<TownDto>>(ConstantsCachingServer.CacheTownsListKey, async (cancellationToken) =>
        {
            var res = await townRepo.GetAllAsync(cancellationToken);
            return res.Select(x => mapper.Map<TownDto>(x)).ToList();
            /* since here no cards info ,so cant do below.instead do at different places
            townDtosList.ForEach(town =>
            {
                if (town.VerifiedCards?.Count > 0 || town.DraftCards?.Count > 0)
                {
                    var maxVerifiedDate = town.VerifiedCards?.Count > 0 ?
                    town.VerifiedCards.Max(c => c.LastModified ?? c.Created) : DateTime.MinValue;

                    var maxDraftDate = town.DraftCards?.Count > 0 ?
                    town.DraftCards.Max(c => c.LastModified ?? c.Created) : DateTime.MinValue;

                    town.LastModified = new[] { maxVerifiedDate, maxDraftDate }.Max();
                }
            });
            return townDtosList.OrderByDescending(t => t.LastModified).ToList();
            */
            //it just returns out of loop
        });

        if (!ServerCachingServiceTowns._isCacheLoaded && townsAll?.Count > 0)
            {
            townsAll.ForEach(x => { x.DraftCards = null; x.VerifiedCards = null; });//unnecessary so take out
            _cachingService.ManualUpdateTownsDictionary(townsAll);
            }
        return townsAll != null ? BaseResult<List<TownDto>>.Ok(townsAll, cacheSetTime, ConstantsCachingServer.TownsAll_MinCacheTimeSpan)
            : new Error(ErrorCode.NotFound, $"Towns Details not found()");
        //below wont work bcz of conversion in TownVerified to TownCardDto
        //return res.Select(r => r.To<Town, TownDto>()).ToList();
        }
    }

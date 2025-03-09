//using Microsoft.Extensions.Logging;
namespace CleanArchitecture.Application.Features.Towns.Queries;

public class GetTownsByNameQueryHandler(ITownRepository TownRepository//, IMapper mapper
     , ServerCachingServiceTowns _cachingService
    //,ILogger<GetTownByIdQueryHandler> logger
    //ITranslator translator, IAuthenticatedUserService authenticatedUser
    ) : IRequestHandler<GetTownsByNameQuery, BaseResult<List<TownDto>>>
    {
    public async Task<BaseResult<List<TownDto>>> Handle(GetTownsByNameQuery request, CancellationToken cancellationToken)
        {//most using ,each town homepage
        var timeNow = DateTimeExtension.CurrentTime;
        (IList<TownDto>? towns, DateTime? cacheSetTime) = await _cachingService.GetOrSetAsync<IList<TownDto>>(ConstantsCachingServer.CacheCardsOfTownNameKey(request.TownName), async (cancellationToken) =>
        {
            return await TownRepository.GetByNameAsync(request.TownName, cancellationToken);
            //it just returns out of loop
        }); // Adjust expiration as needed,otherwise default cache of 6hr/2hr sliding
        return towns != null ? BaseResult<List<TownDto>>.Ok(towns!.ToList(), cacheSetTime, ConstantsCachingServer.Town_MinCacheTimeSpan)
           : new Error(ErrorCode.NotFound, $"Town({request.TownName}) Details not found()");
        //if (town is null)
        //    {
        //    return new Error(ErrorCode.NotFound, $"Town({request.IdTown}) Details not found()", nameof(request.IdTown));
        //    }
        // var result = mapper.Map<TownDto>(town);
        //below wont work bcz of conversion in TownVerified to TownCardDto
        //var result = town.To<Town, TownDto>();

        //if (town.ServerSideFromDbLoadedTime == timeNow)
        //    logger.LogWarning("From db loaded");
        //else logger.LogWarning("From cache");
        // town.CacheLoadedTime = timeNow;

        //return town;
        }
    }

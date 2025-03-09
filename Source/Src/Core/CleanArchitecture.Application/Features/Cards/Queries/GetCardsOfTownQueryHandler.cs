//using Microsoft.Extensions.Logging;
namespace CleanArchitecture.Application.Features.Cards.Queries;

public class GetCardsOfTownQueryHandler(ICardRepository cardRepository//, IMapper mapper
     , ServerCachingServiceTowns _cachingService
    //,ILogger<GetCardsOfTownQueryHandler> logger
    //ITranslator translator, IAuthenticatedUserService authenticatedUser
    ) : IRequestHandler<GetCardsOfTownQuery, BaseResult<TownCardsDto>>
    {
    public async Task<BaseResult<TownCardsDto>> Handle(GetCardsOfTownQuery request, CancellationToken cancellationToken)
        {//most using ,each town homepage
        if (request.IdTown == 0)
            throw new Exception("Town Id is Zero 0");

        if (request.RefreshByAdmin)
            _cachingService.Invalidate(ConstantsCachingServer.CacheCardsOfTownIdKey(request.IdTown));

        //var timeNow = DateTimeExtension.CurrentTime;
        (TownCardsDto? town, DateTime? cacheSetTime) = await _cachingService.GetOrSetAsync<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(request.IdTown), async (cancellationToken) =>
        {
            var townFromDb = await cardRepository.GetCardsOfTown(request.IdTown, cancellationToken);
            //townFromDb.ServerSideFromDbLoadedTime = timeNow;//sets only for dbData loaded
            return townFromDb;
            //it just returns out of loop
        }); // Adjust expiration as needed,otherwise default cache of 6hr/2hr sliding
        return town != null ? BaseResult<TownCardsDto>.Ok(town, cacheSetTime, ConstantsCachingServer.Town_MinCacheTimeSpan)
           : new Error(ErrorCode.NotFound, $"Town({request.IdTown}) Details not found()");
        }
    }

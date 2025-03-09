namespace CleanArchitecture.Application.Features.CardTypes.Queries
    {
    public class GetCardTypesAllQueryHandler(ICardTypeRepository cardTypeRepository, IMapper mapper, ServerCachingServiceTowns _cachingService) : IRequestHandler<GetCardTypesAllQuery, BaseResult<List<CardTypeDto>>>
        {
        public async Task<BaseResult<List<CardTypeDto>>> Handle(GetCardTypesAllQuery request, CancellationToken cancellationToken)
            {
            (List<CardTypeDto>? cardTypesAll, DateTime? cacheSetTime)
                = await _cachingService.GetOrSetAsync<List<CardTypeDto>>(ConstantsCachingServer.CacheCardTypesListKey, async (cancellationToken) =>
            {
                var res = await cardTypeRepository.GetAllAsync(cancellationToken);
                return res.Select(x => mapper.Map<CardTypeDto>(x)).ToList();
                //it just returns out of loop
            }, cancellationToken: cancellationToken);
            //return cardTypesAll != null ? cardTypesAll : new Error(ErrorCode.NotFound, $"Town Card_Draft Types Details not found()");
            return cardTypesAll != null ? BaseResult<List<CardTypeDto>>.Ok(cardTypesAll, cacheSetTime, ConstantsCachingServer.CardTypesAll_MinCacheTimeSpan)
          : new Error(ErrorCode.NotFound, $"Town Card Types Details All Unable to extract");
            }
        }
    }

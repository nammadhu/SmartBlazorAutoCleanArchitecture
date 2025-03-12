namespace CleanArchitecture.Application.Features.Cards.Commands;

public class ApproveCardCommandHandler(ICardRepository townCardRepo, ServerCachingTownCards _cachingService, IMapper mapper, ILogger<ApproveCardCommandHandler> logger, ITranslator translator)
    : IRequestHandler<ApproveCardCommand, BaseResult<bool>>
    {
    public async Task<BaseResult<bool>> Handle(ApproveCardCommand request, CancellationToken cancellationToken)
        {
        try
            {
            (bool approvedResult, int townIdRefreshRequired) = await townCardRepo.ApproveCardAsync(request, cancellationToken);

            if (approvedResult)
                {
                (TownCardsDto? cacheTown, DateTime? cacheSetTime) = _cachingService.Get<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(townIdRefreshRequired));
                if (cacheTown != null)//if no cache then no need to update
                    {
                    //fetch card,data,detail and then updateCard of town
                    var card = await townCardRepo.GetByIdIntAsync(request.IdCard, cancellationToken);
                    var cardDto = mapper.Map<CardDto>(card);
                    _cachingService.AddOrUpdateCardInTown(townIdRefreshRequired, cardDto);
                    }
                }
            return BaseResult<bool>.OkNoClientCaching(approvedResult);
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in iCard Approval"));
            }
        }
    }

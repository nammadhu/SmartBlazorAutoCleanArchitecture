namespace CleanArchitecture.Application.Features.Cards.Commands;

public class CardDetailOpenCloseUpdateCommandHanlder(
    ICardDetailRepository cardDetailsRepository, ITranslator translator,
    ServerCachingTownCards cachingServiceTown, ILogger<CU_CardDetailCommand> logger)
    : IRequestHandler<CardDetailOpenCloseUpdateCommand, BaseResult<bool?>>
//IUnitOfWork unitOfWork,  IMapper mapper,
    {
    public async Task<BaseResult<bool?>> Handle(CardDetailOpenCloseUpdateCommand request, CancellationToken cancellationToken)
        {
        try
            {
            if (request == null) return default;

            //s1.check in cache if already same then return true
            //s2.else update  in db
            //s3.then check in cache if exists then update there also
            //s4.if not exists just leave it. mostly always it wont be the case

            //s1 check in cache as changes required or not
            iCardDto? existingFullCard = null;
            (TownCardsDto? townCache, DateTime? cacheSetTime) = cachingServiceTown.Get<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(request.IdTown));
            if (townCache?.VerifiedCards != null && townCache.VerifiedCards.Exists(x => x.Id == request.Id))
                existingFullCard = townCache.VerifiedCards.FirstOrDefault(x => x.Id == request.Id);
            else if (townCache?.DraftCards != null && townCache.DraftCards.Exists(x => x.Id == request.Id))
                existingFullCard = townCache.DraftCards.FirstOrDefault(x => x.Id == request.Id);

            if (existingFullCard?.CardDetail != null && existingFullCard.CardDetail.IsOpenNow == request.IsOpenClose)
                return true;//already same in cache itself so no more changes required

            //s2
            int resultCount = await cardDetailsRepository.UpdateOpenClose(request.Id, request.IsOpenClose, request.Operator, cancellationToken);
            if (resultCount > 0)//success now updated
                {
                if (existingFullCard?.CardDetail != null)
                    { //s3 update
                    existingFullCard.CardDetail.IsOpenNow = request.IsOpenClose;
                    existingFullCard.CardDetail.LastModified = DateTimeExtension.CurrentTime;
                    existingFullCard.CardDetail.LastModifiedBy = request.Operator;

                    existingFullCard.Id = request.Id;//may be not required but better to keep
                    cachingServiceTown.AddOrUpdateCardInTown(request.IdTown, cardDetail: existingFullCard.CardDetail);
                    }
                //else //s4 just leave it out
                return true;
                }
            return false;
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in iCard Detail /UpdateCardOnly"));
            }
        }
    }

namespace CleanArchitecture.Application.Features.Cards.Commands;

public class CU_CardDataCommandHandler(
    ICardDataRepository cardDataRepository,
    IUnitOfWork unitOfWork, ITranslator translator,
    ServerCachingTownCards cachingServiceTown
    , ILogger<CU_CardDataCommandHandler> logger)
    : IRequestHandler<CU_CardDataCommand, BaseResult<CardData>>
    {
    public async Task<BaseResult<CardData>> Handle(CU_CardDataCommand request, CancellationToken cancellationToken)
        {
        try
            {
            iCardDto? existingFullCard = null;
            (TownCardsDto? townCache, DateTime? cacheSetTime) = cachingServiceTown.Get<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(request.IdTown));
            if (townCache?.VerifiedCards != null && townCache.VerifiedCards.Exists(x => x.Id == request.Id))
                existingFullCard = townCache.VerifiedCards.FirstOrDefault(x => x.Id == request.Id);
            else if (townCache?.DraftCards != null && townCache.DraftCards.Exists(x => x.Id == request.Id))
                existingFullCard = townCache.DraftCards.FirstOrDefault(x => x.Id == request.Id);

            if (request == null) return null;
            CardData? existingCardData = existingFullCard?.CardData;
            if (existingCardData == null)
                existingCardData = await cardDataRepository.GetByIdAsync(request.Id, cancellationToken);//this can be null also ,so below metho makes as Add

            if (existingCardData.IdTown == 0)
                existingCardData.IdTown = existingFullCard?.IdTown ?? 0;
            (bool updateRequired, bool addRequired) = CardData.UpdateExistingDbEntity(existingCardData, request);
            if (addRequired || updateRequired)
                {
                CardData? updatedResult = null;
                if (addRequired)
                    updatedResult = await cardDataRepository.AddAsync(existingCardData, cancellationToken);
                else
                    cardDataRepository.Update(existingCardData);
                if (updateRequired) updatedResult = existingCardData;

                if (await unitOfWork.SaveChangesAsync(cancellationToken) && updatedResult != null)
                    {//success go for cache update and return
                    existingFullCard ??= new();
                    existingFullCard.Id = request.Id;//may be not required but better to keep
                    existingFullCard.CardData = updatedResult;
                    cachingServiceTown.AddOrUpdateCardInTown(request.IdTown, cardData: updatedResult);

                    updatedResult.IsVerified = existingFullCard.IsVerified;
                    return updatedResult;
                    }
                throw new Exception($"{nameof(CardData)} adding failed for idCard:{request.Id},idTown:{request.IdTown}");
                }
            else//nothing just ignore and return,as mostly no change required to make
                return existingCardData;
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in iCard UpdateCardOnly CardData"));
            }
        }
    }

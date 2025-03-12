using BASE;
using BASE.Common;

namespace CleanArchitecture.Application.Features.Cards.Commands;

public class CU_CardDetailCommandHandler(
    ICardDetailRepository cardDetailsRepository, IAzImageStorage azImageStorage,
    IUnitOfWork unitOfWork, ITranslator translator, IMapper mapper,
    ServerCachingTownCards cachingServiceTown
    , ILogger<CU_CardDetailCommand> logger)
    : IRequestHandler<CU_CardDetailCommand, BaseResult<CardDetailDto>>
    {
    public async Task<BaseResult<CardDetailDto>> Handle(CU_CardDetailCommand request, CancellationToken cancellationToken)
        {
        try
            {
            CardDto? existingFullCard = null;
            (TownCardsDto? townCache, DateTime? cacheSetTime) = cachingServiceTown.Get<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(request.IdTown));
            if (townCache?.VerifiedCards != null && townCache.VerifiedCards.Exists(x => x.Id == request.Id))
                existingFullCard = townCache.VerifiedCards.FirstOrDefault(x => x.Id == request.Id);
            else if (townCache?.DraftCards != null && townCache.DraftCards.Exists(x => x.Id == request.Id))
                existingFullCard = townCache.DraftCards.FirstOrDefault(x => x.Id == request.Id);

            if (request == null || CardDetailDto.IsNullOrDefault(request))
                return null;
            CardDetail? existingCardDetailDomain = existingFullCard?.CardDetail;
            if (existingCardDetailDomain == null)
                existingCardDetailDomain = await cardDetailsRepository.GetByIdAsync(request.Id, cancellationToken);//this can be null also ,so below metho makes as Add

            (bool updateRequired, bool addRequired) = CardDetailDto.UpdateExistingDbEntity(existingCardDetailDomain, request);
            List<ImageInfo> listToUpload1 = new();
            List<ImageInfo> listToDelete1 = new();
            bool imageChangesExists = DetailsImagesUpdatesNew(existingCardDetailDomain, request, ConstantsTown.CardDetailImagesPrefix, listToUpload1, listToDelete1);
            if (addRequired || updateRequired || imageChangesExists)
                {
                if (imageChangesExists)
                    {
                    if (ListExtensions.HasData(listToUpload1) || ListExtensions.HasData(listToDelete1))
                        {
                        var existingCardToUpdate = mapper.Map<CardDto, Card>(existingFullCard);
                        existingCardToUpdate.CardDetail = existingCardDetailDomain;
                        await ImageUploadOrDelete(existingCardToUpdate, ConstantsTown.CardMainImagesPrefix, ConstantsTown.CardDetailImagesPrefix, listToUpload1, listToDelete1, cancellationToken);
                        existingCardDetailDomain = existingCardToUpdate.CardDetail;
                        }
                    }
                CardDetail? updatedResult = null;
                if (addRequired)
                    updatedResult = await cardDetailsRepository.AddAsync(existingCardDetailDomain, cancellationToken);
                else
                    cardDetailsRepository.Update(existingCardDetailDomain);
                if (updateRequired) updatedResult = existingCardDetailDomain;

                if (await unitOfWork.SaveChangesAsync(cancellationToken) && updatedResult != null)
                    {//success go for cache update and return
                    existingFullCard ??= new();
                    existingFullCard.Id = request.Id;//may be not required but better to keep
                    existingFullCard.CardDetail = mapper.Map<CardDetailDto>(updatedResult);
                    cachingServiceTown.AddOrUpdateCardInTown(request.IdTown, cardDetail: existingFullCard.CardDetail);
                    var result = mapper.Map<CardDetailDto>(updatedResult);
                    result.IdTown = existingFullCard.IdTown;
                    updatedResult.IsVerified = existingFullCard.IsVerified;
                    return result;
                    }
                throw new Exception($"{nameof(CardData)} adding failed for idCard:{request.Id},idTown:{request.IdTown}");
                }
            else//nothing just ignore and return,as mostly no change required to make
                {
                var result = mapper.Map<CardDetailDto>(existingCardDetailDomain);
                if (result.IdTown == 0)
                    result.IdTown = existingFullCard?.IdTown ?? 0;
                result.IsVerified = existingFullCard?.IsVerified;
                return result;
                }
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in iCard Detail /UpdateCardOnly"));
            }
        }

    private bool DetailsImagesUpdatesNew(CardDetail existingCardDetail, CardDetailDto newCardDetailDto,
       string cardDetailImagesPrefix, List<ImageInfo> listToUpload, List<ImageInfo> listToDelete)
        {
        if (CardDetailDto.IsNotImageUrlNorBase64(newCardDetailDto) && CardDetail.IsNotImageUrls(existingCardDetail) ||
            CardDetailDto.IsNotImageUrlNorBase64(newCardDetailDto) && !CardDetail.IsNotImageUrls(existingCardDetail))
            {
            //nothing just ignore,no difference
            return false;
            }
        else if (newCardDetailDto != null && !(CardDetailDto.IsNotImageUrlNorBase64(newCardDetailDto) && CardDetail.IsNotImageUrls(existingCardDetail)))
            {//todo also one more like both are equal then also skip
            //request.CardDetail??= new();//means adding new
            var (listToUploadProduct, listToDeleteProduct) = newCardDetailDto.GetUploadAndDeleteImages(existingCardDetail);
            if (ListExtensions.HasData(listToUploadProduct))
                {
                listToUploadProduct.ForEach(x => x.ImageName = cardDetailImagesPrefix + x.ImageName ?? "");
                listToUpload.AddRange(listToUploadProduct);
                }
            if (ListExtensions.HasData(listToDeleteProduct))
                {
                listToDeleteProduct.ForEach(x => x.ImageName = cardDetailImagesPrefix + x.ImageName ?? "");
                listToDelete.AddRange(listToDeleteProduct);
                }
            return true;
            }
        return false;
        }

    private async Task ImageUploadOrDelete(Card exCardToUpdate, string brandPrefix, string productPrefix, List<ImageInfo> listToUpload, List<ImageInfo> listToDelete, CancellationToken cancellationToken)
        {
        if (ListExtensions.HasData(listToUpload) || ListExtensions.HasData(listToDelete))
            {
            (List<ImageInfo> imagesUploaded, List<ImageInfo> imagesDeleted) = await azImageStorage.DeleteAndUploadImagesToCardId(exCardToUpdate.Id, listToUpload, listToDelete, cancellationToken);

            var brandImagesUploadedResult = imagesUploaded?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(brandPrefix))?.ToList();
            var productImagesUploadedResult = imagesUploaded?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(productPrefix))?.ToList();

            var brandImagesDeletedResult = imagesDeleted?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(brandPrefix))?.ToList();
            var productImagesDeletedResult = imagesDeleted?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(productPrefix))?.ToList();
            //delete first then upload entry,so replacement all handled properly
            //cardBrand images
            if (brandImagesDeletedResult != null && ListExtensions.HasData(brandImagesDeletedResult))
                foreach (var imageInfo in brandImagesDeletedResult)
                    {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(brandPrefix, string.Empty);
                    ImageInfo matched = brandImagesDeletedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (matched != null && matched.DeletedStatus == true && imageInfo.ImageName != null)
                        exCardToUpdate.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exCardToUpdate, null);
                    }
            if (brandImagesUploadedResult != null && ListExtensions.HasData(brandImagesUploadedResult))
                foreach (var imageInfo in brandImagesUploadedResult)
                    {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(brandPrefix, string.Empty);
                    ImageInfo matched = brandImagesUploadedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (matched != null && imageInfo.ImageName != null)
                        exCardToUpdate.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exCardToUpdate, matched?.Url);
                    }
            //Detail images
            if (productImagesDeletedResult != null && ListExtensions.HasData(productImagesDeletedResult))
                foreach (var imageInfo in productImagesDeletedResult)
                    {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(productPrefix, string.Empty);
                    ImageInfo matched = productImagesDeletedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (exCardToUpdate.CardDetail != null && matched != null && matched.DeletedStatus == true && imageInfo.ImageName != null)
                        exCardToUpdate.CardDetail.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exCardToUpdate.CardDetail, null);
                    }
            if (productImagesUploadedResult != null && ListExtensions.HasData(productImagesUploadedResult))
                foreach (var imageInfo in productImagesUploadedResult)
                    {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(productPrefix, string.Empty);
                    ImageInfo matched = productImagesUploadedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (exCardToUpdate.CardDetail != null && matched != null && imageInfo.ImageName != null)
                        exCardToUpdate.CardDetail.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exCardToUpdate.CardDetail,
                        matched?.Url);
                    }
            }
        }
    }

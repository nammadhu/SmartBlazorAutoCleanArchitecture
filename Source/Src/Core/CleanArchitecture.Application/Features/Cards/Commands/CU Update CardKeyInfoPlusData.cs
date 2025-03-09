using MediatR;
using Microsoft.Extensions.Logging;
using MyTown.SharedModels;
using MyTown.SharedModels.Features.Cards.Commands;
using PublicCommon.Common;
using SharedResponse;
using System.Threading;

namespace MyTown.Application.Features.Cards.Commands;

public partial class CreateUpdateCardCommandHandler
{
    /*
    async Task<iCardDto?> CardKeyInfoPlusDataUpdate(CreateUpdateCardCommand request, iCardDto? existingFullCard, CancellationToken cancellationToken)
    {
        //always update is either Draft or Draft+Verified for KeyInfo changes. Never direct to Verified

        //if existingFullCard is null then fetch on demand basis as below
        //it can also be null when user directly accessing the card page
        if (request.IsKeyInfoChanged == true && request.IsDataChanged == false)
        {
            if (request.IdCardType == 0 || string.IsNullOrEmpty( request.Title))
                return existingFullCard;
            CardKeyDraft? existingCardDomain = mapper.Map<CardKeyDraft>(existingFullCard);
            if (existingCardDomain == null)
                existingCardDomain = await draftCardKeyRepository.GetByIdAsync(request.Id, cancellationToken);//this can be null also ,so below metho makes as Add

            (bool updateRequired, bool addRequired) = CardDetailDto.UpdateExistingDbEntity(existingCardDomain, request.CardDetail);
            List<ImageInfo> listToUpload1 = new();
            List<ImageInfo> listToDelete1 = new();
            bool imageChangesExists = DetailsImagesUpdatesNew(existingCardDomain, request.CardDetail, ConstantsTown.CardDetailImagesPrefix, listToUpload1, listToDelete1);
            try
            {
                if (addRequired || updateRequired || imageChangesExists)
                {
                    if (imageChangesExists)
                    {
                        if (ListExtensions.HasData(listToUpload1) || ListExtensions.HasData(listToDelete1))
                        {
                            var existingCardToUpdate = mapper.Map<iCardDto, CardKeyDraft>(existingFullCard);
                            existingCardToUpdate.CardDetail = existingCardDomain;
                            await ImageUploadOrDelete(existingCardToUpdate, ConstantsTown.CardMainImagesPrefix, ConstantsTown.CardDetailImagesPrefix, listToUpload1, listToDelete1, cancellationToken);
                            existingCardDomain = existingCardToUpdate.CardDetail;
                        }
                    }
                    CardDetail? updatedResult = null;
                    if (addRequired)
                        updatedResult = await cardDetailsRepository.AddAsync(existingCardDomain, cancellationToken);
                    else
                        cardDetailsRepository.UpdateCard(existingCardDomain);
                    if (updateRequired) updatedResult = existingCardDomain;

                    if (await unitOfWork.SaveChangesAsync(cancellationToken) && updatedResult != null)
                    {//success go for cache update and return 
                        existingFullCard ??= new();
                        existingFullCard.Id = request.Id;//may be not required but better to keep
                        existingFullCard.CardDetail = mapper.Map<CardDetailDto>(updatedResult);
                        cachingServiceTown.AddOrUpdateCardInTown(request.IdTown, cardDetail: existingFullCard.CardDetail);
                        return existingFullCard;
                    }
                    throw new Exception($"{nameof(CardData)} adding failed for idCard:{request.Id},idTown:{request.IdTown}");
                }
                else//nothing just ignore and return,as mostly no change required to make
                    return existingFullCard;
            }
            catch (Exception ex)
            {
                logger.LogError(exception: ex, message: "CardData UpdateCard Failed");
            }
        }
        return existingFullCard;
    }
    */
}
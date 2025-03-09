using MediatR;
using Microsoft.Extensions.Logging;
using MyTown.SharedModels;
using MyTown.SharedModels.Features.Cards.Commands;
using PublicCommon.Common;
using SharedResponse;
using System.Collections.Generic;
using System.Threading;

namespace MyTown.Application.Features.Cards.Commands;

public partial class CreateUpdateCardCommandHandler
{
    /*
    async Task<BaseResult<iCardDto?>> CardKeyInfoUpdate(CreateUpdateCardCommand request, iCardDto? existingFullCard, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(authenticatedUser.UserId, out Guid userId)) 
            return BaseResult<iCardDto>.Failure();
        //always update is either Draft or Draft+Verified for KeyInfo changes. Never direct to Verified

        //if existingFullCard is null then fetch on demand basis as below
        //it can also be null when user directly accessing the card page
        if (request.IsKeyInfoChanged == true && request.IsDataChanged == false)
        {
            if (request.IdCardType == 0 || string.IsNullOrEmpty(request.Title))
                return existingFullCard;
            CardKeyDraft? existingCard = mapper.Map<CardKeyDraft>(existingFullCard);
            if (existingCard == null)
                existingCard = await draftCardKeyRepository.GetByIdAsync(request.Id, cancellationToken);//this can be null also ,so below metho makes as Add
            var cardToUpdate = existingCard.CloneBySerializing();
            if (cardToUpdate == null) throw new Exception("existingCard Clone Failed");
            (bool updateRequired, bool addRequired) = CreateUpdateCardCommand.UpdateExistingDbEntity(existingEntityForUpdate:cardToUpdate, request);
            List<ImageInfo> listToUpload1 = new();
            List<ImageInfo> listToDelete1 = new();
            bool imageChangesExists = MainImagesUpdate(request, existingCard: existingCard, ConstantsTown.CardMainImagesPrefix, listToUpload1, listToDelete1);
            
            try
            {
                if (addRequired || updateRequired || imageChangesExists)
                {
                    if (imageChangesExists)
                    {
                        if (ListExtensions.HasData(listToUpload1) || ListExtensions.HasData(listToDelete1))
                        {
                            //if draft image has been used by verified,then delete should not be the case
                            if (existingCard.IsVerified == true && ListExtensions.HasData(listToDelete1))
                            {
                                List<ImageInfo> newListToDelete = [];// listToDelete.CloneBySerializing();
                                listToDelete1.ForEach(x =>
                                {
                                    if ((x.ImageName == ConstantsTown.CardMainImagesPrefix + nameof(existingCard.VerifiedCard.Image1) && x.Url == existingCard.VerifiedCard?.Image1)
                                    || (x.ImageName == ConstantsTown.CardMainImagesPrefix + nameof(existingCard.VerifiedCard.Image2) && x.Url == existingCard.VerifiedCard?.Image2))
                                    { }
                                    else newListToDelete.Add(x);
                                });
                                listToDelete1 = newListToDelete;
                            }


                            //var existingCardToUpdate = mapper.Map<iCardDto, CardKeyDraft>(existingFullCard);
                            //existingCardToUpdate.CardDetail = existingCardDomain;

                            //todo rework
                            //await ImageUploadOrDelete(existingCardDomain,ConstantsTown.CardMainImagesPrefix,ConstantsTown.CardDetailImagesPrefix, listToUpload1, listToDelete1, cancellationToken);
                        }
                    }
                    //todo check if difference then only update otherwise skip... implement different or not
                    bool pendingSubmit;
                    if (existingCard.CardApprovals?.Count > 0 || request.CardApprovals?.Count > 0)
                        pendingSubmit = await AddUpdateApproverVerifiers(request.Id, existingCard.CardApprovals?.ToList(), request.CardApprovals, cancellationToken);

                    bool cardMoveToVerifiedCards = (request.IsForVerifiedCard == true) &&
                        existingCard.IsVerified != true;

                    if (existingCard.IsVerified == true || cardToUpdate.IsSameAsVerified == true)
                    {
                        cardToUpdate.IsSameAsVerified = (request.IsForVerifiedCard == true);
                        cardToUpdate.IsVerified = (request.IsForVerifiedCard == true);
                    }
                    cardToUpdate.LastModified = DateTimeExtension.CurrentTime;
                    draftCardKeyRepository.UpdateCard(cardToUpdate);
                    //pendingSubmit = true;
                    bool isUpdateSuccess;
                    bool townCardCacheRefreshRequired;
                    if (cardMoveToVerifiedCards)
                    {
                        (bool approvedResult, int townIdRefreshRequired) = await draftCardKeyRepository.ApproveCardByAdmin
                            (new ApproveCardCommand()
                            {
                                IdCard = cardToUpdate.Id,
                                IdApproverCard = cardToUpdate.IdTown,
                                IdApprover = userId,
                                ApproveStatus = true
                            },
                                cancellationToken, cardToUpdate, mainDataChanged);
                        //above calls dbSaveChanges,so no more changes required
                        pendingSubmit = false;
                        isUpdateSuccess = approvedResult;
                        townCardCacheRefreshRequired = true;
                    }
                    else if (pendingSubmit && request.IsForVerifiedCard == true) townCardCacheRefreshRequired = true;





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
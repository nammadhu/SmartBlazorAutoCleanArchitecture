using MyTown.SharedModels.Features.Cards.Commands;
using PublicCommon.Common;
using Shared;

namespace MyTown.Application.Features.Cards.Commands;

public partial class CreateUpdateCardCommandHandler
{
    private async Task<BaseResult<iCardDto>> Update(CreateUpdateCardCommand request, CancellationToken cancellationToken)
    {
      if (!Guid.TryParse(authenticatedUser.UserId, out Guid userId)) return BaseResult<iCardDto>.Failure();
        var existingCard = await cardRepository.GetDraftCardByIdIntIncludeDetailsAsync(request.Id, cancellationToken);
        if (existingCard == null)
        {
            return new Error(ErrorCode.NotFound, $"Cant update Non-Existing card with id {request.Id}", nameof(request.Id));
        }
        var cardToUpdate = existingCard.CloneBySerializing();
        if (cardToUpdate == null) throw new Exception("existingCard Clone Failed");
        //CARD has 3 sections
        //1.card with image1,image2,name,subtitle
        //2.CardData having fbUrls,phone numbers,flexible data to change
        //3.CardDetail which appears on full screen iCard/id

        //Type1:while updating card(1),it sends card(1) & cardData(2) together. here don't update cardDetail(3)
        //Type2:while updating cardDetail(3) separately, it sends only cardDetail(3),so don't update card(1) or cardData(2)

        List<ImageInfo> listToUpload = new();
        List<ImageInfo> listToDelete = new();
        bool pendingSubmit = false;
        bool townCardCacheRefreshRequired = false;
        CardData? cardDataToUpdateInCache = null;
        CardDetailDto? cardDetailToUpdateInCache = null;
        bool isUpdateSuccess = false;

        if (CardDetailDto.IsNullOrDefault(request.CardDetail) && request.CardData != null)
        {//means Type1 cardUpdate with data
         //retain cardDetail as it is

            //instead of copying one by one properties,copied all at a time using mapping
            mapper.Map<CreateUpdateCardCommand, CardKeyDraft>(request, cardToUpdate);
            //above makes isVerified as null and all other values overwritten,so manual copy as below
            cardToUpdate.IsVerified = existingCard.IsVerified;
            cardToUpdate.CardDetail = existingCard.CardDetail;//may be had to tune this,otherwise this might add 1 more extra save

            //main images(only),had to check difference
            MainImagesUpdate(request, existingCard, cardToUpdate, ConstantsTown.CardMainImagesPrefix, listToUpload, listToDelete);
            if (ListExtensions.HasData(listToUpload) || ListExtensions.HasData(listToDelete))
            {
                //if draft image has been used by verified,then delete should not be the case
                if (existingCard.IsVerified == true && ListExtensions.HasData(listToDelete))
                {
                    List<ImageInfo> newListToDelete = [];// listToDelete.CloneBySerializing();
                    listToDelete.ForEach(x =>
                    {
                        if ((x.ImageName == ConstantsTown.CardMainImagesPrefix + nameof(existingCard.VerifiedCard.Image1) && x.Url == existingCard.VerifiedCard?.Image1)
                        || (x.ImageName == ConstantsTown.CardMainImagesPrefix + nameof(existingCard.VerifiedCard.Image2) && x.Url == existingCard.VerifiedCard?.Image2))
                        { }
                        else newListToDelete.Add(x);
                    });
                    listToDelete = newListToDelete;
                }

                await ImageUploadOrDelete(cardToUpdate, ConstantsTown.CardMainImagesPrefix, ConstantsTown.CardDetailImagesPrefix, listToUpload, listToDelete, cancellationToken);
            }
            //todo check if difference then only update otherwise skip... implement different or not
            if (existingCard.CardApprovals?.Count > 0 || request.CardApprovals?.Count > 0)
                pendingSubmit = await AddUpdateApproverVerifiers(request.Id, existingCard.CardApprovals?.ToList(), request.CardApprovals, cancellationToken);

            //card main data changes only if changes exists
            bool mainDataChanged = cardToUpdate.IsCardMainDataChanged(existingCard);
            bool cardMoveToVerifiedCards = (request.IsForVerifiedCard == true) &&
                (mainDataChanged || existingCard.IsVerified != true);

            if (mainDataChanged)//mainData changed
            {
                if (existingCard.IsVerified == true || cardToUpdate.IsSameAsVerified == true)
                {
                    cardToUpdate.IsSameAsVerified = (request.IsForVerifiedCard == true);
                    cardToUpdate.IsVerified = (request.IsForVerifiedCard == true);
                }
                cardToUpdate.LastModified = DateTimeExtension.CurrentTime;
                cardRepository.Update(cardToUpdate);
                pendingSubmit = true;
            }
            //else
            //    {//NO mainData changed
            //    if (existingCard.IsVerified != true && request.IsForVerifiedCard == true)
            //        { //card not verified & now admin approved so go for approve card by admin
            //        }
            //    //else already approved no more changes required
            //    }

            if (cardToUpdate.CardData != null)
            {
                cardToUpdate.CardData.Id = existingCard.CardData?.Id ?? 0;
                if (!cardToUpdate.CardData.Equals(existingCard.CardData))
                {
                    cardToUpdate.LastModified = DateTimeExtension.CurrentTime;
                    cardDataRepository.Update(cardToUpdate.CardData);
                    pendingSubmit = true;
                    townCardCacheRefreshRequired = true;
                    cardDataToUpdateInCache = cardToUpdate.CardData;
                }
            }
            else
            {
                cardToUpdate.CardData = existingCard.CardData;//this is required for returning result;
            }
            //var result1 = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (cardMoveToVerifiedCards)
            {
                (bool approvedResult, int townIdRefreshRequired) = await cardRepository.ApproveCardByAdmin
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
        }
        else //type2 cardDetail update only
        { //retain card & cardData as it is
            request.CardDetail ??= new();//todo handle this case
            cardToUpdate.CardDetail ??= new();//todo handle this case
            request.CreatedBy = cardToUpdate.CreatedBy;//need to retain so
            mapper.Map<CardDetailDto, CardDetail>(request.CardDetail, cardToUpdate.CardDetail);
            DetailsImagesUpdates(request, existingCard, cardToUpdate, ConstantsTown.CardDetailImagesPrefix, listToUpload, listToDelete);
            if (ListExtensions.HasData(listToUpload) || ListExtensions.HasData(listToDelete))
                await ImageUploadOrDelete(cardToUpdate, ConstantsTown.CardMainImagesPrefix, ConstantsTown.CardDetailImagesPrefix, listToUpload, listToDelete, cancellationToken);
            if (cardToUpdate.CardDetail != null)
            {
                cardToUpdate.CardDetail.Id = existingCard.CardDetail?.Id ?? 0;
                cardToUpdate.LastModified = DateTimeExtension.CurrentTime;
                cardDetailsRepository.Update(cardToUpdate.CardDetail);
                pendingSubmit = true;
            }
            else
            {
                cardToUpdate.CardDetail = existingCard.CardDetail;//this is required for returning result;
            }
            townCardCacheRefreshRequired = true;
            cardDetailToUpdateInCache = mapper.Map<CardDetailDto>(cardToUpdate.CardDetail);
        }

        // existingCard.ModifiedBy = authenticatedUser.UserId; // Assign modified user ID
        if (pendingSubmit)
            isUpdateSuccess = await unitOfWork.SaveChangesAsync(cancellationToken);

        if ((existingCard.IsVerified == true || cardToUpdate.IsVerified == true) && isUpdateSuccess)
        {//currently only update cache for verified cards,not for drafts
            //currently refresh only to verified,if draft required then tweak here
            if (townCardCacheRefreshRequired)
            {//mark only for detail/data/image changes ,not for draft changes
             // Start the task without waiting for completion
             //_ = Task.Run(() => backgroundJobsRepository.MarkTownAsCardsUpdated(request.IdTown, cancellationToken));
             //above making problem of 2nd operation started kind of failures,so avoiding
                cachingServiceTown.AddOrUpdateCardInTown(request.IdTown, mapper.Map<iCardDto>(cardToUpdate), cardDataToUpdateInCache, cardDetailToUpdateInCache, isVerified: cardToUpdate.IsVerified ?? false, townCardsModifiedTime: cardToUpdate.LastModified ?? DateTimeExtension.CurrentTime);
            }
            return BaseResult<iCardDto>.OkNoClientCaching(mapper.Map<iCardDto>(cardToUpdate) ?? new());
        }
        //no changes,so return original as it is
        return BaseResult<iCardDto>.OkNoClientCaching(mapper.Map<iCardDto>(existingCard) ?? new());
    }

    private async Task<bool> AddUpdateApproverVerifiers(int cardId, List<Domain.CardApproval>? existing, List<Domain.CardApproval>? newSelectedSet, CancellationToken cancellationToken)
    {
        if (newSelectedSet?.Count > 0) newSelectedSet.ForEach(x => x.IdCard = cardId);
        //from frontend it wone send cardId

        if (CardApprovalExtensions.AreListsEqual(existing, newSelectedSet, new CardApprovalComparer())) return false;

        //Note:Clear out self approving or others cardid setting
        //C1:if existing approver null & new null then skip
        //C3:if existing null, then insert all new
        //C4:else if new all null then existing all remove

        //C2:if existing id & current id same list then skip
        //C5:ctoRemove, in existing list removeto , add newones

        //Note:Clear out self approving or others cardid setting
        if (ListExtensions.HasData(newSelectedSet) && newSelectedSet != null)
            newSelectedSet.RemoveAll(x => x.IdCardOfApprover == cardId || x.IdCard != cardId);
        //above means,if same card as approver then remove or other than same card should not be allowed

        //C1:if existing approver null & new null then skip
        if ((existing == null || existing.Count == 0) && (newSelectedSet == null || newSelectedSet.Count == 0))
            return false;

        //C3:if existing null, then insert all new
        if (ListExtensions.IsEmptyList(existing) && ListExtensions.HasData(newSelectedSet))
        {
            newSelectedSet!.ForEach(async n =>
            await townCardApprovalRepository.AddAsync(n, cancellationToken));
            return true;
        }

        //C4:else if new all null then existing all remove
        if (ListExtensions.IsEmptyList(newSelectedSet) && ListExtensions.HasData(existing))
        {
            await townCardApprovalRepository.DeleteCardApprovals(cardId, existing, cancellationToken);
            return true;
        }
        if (ListExtensions.HasData(existing) && existing != null && ListExtensions.HasData(newSelectedSet) && newSelectedSet != null)
        {
            //C2:if existing id & current id same list then skip
            if (CardApprovalExtensions.AreListsEqual(existing, newSelectedSet, new CardApprovalComparer()))
                return false;//already checked above but again after removal of self

            //if (ListExtensions.AreListsEqualIgnoringOrder(existing.Select(x => x.IdCardOfApprover).ToList(), newSelectedSet!.Select(x => x.IdCardOfApprover).ToList()))
            //    return false;

            //C5:ctoRemove, in existing list removeto , add newones
            var forAdding = newSelectedSet.Except(existing);
            var forRemove = existing.Except(newSelectedSet);

            if (ListExtensions.HasData(forRemove))
            {
                //existing.RemoveAll(x => forRemove.Contains(x.IdCardOfApprover));
                await townCardApprovalRepository.DeleteCardApprovals(cardId, forRemove.ToList(), cancellationToken);
            }
            if (ListExtensions.HasData(forAdding))
            {
                forAdding.ToList().ForEach(async a => await townCardApprovalRepository.
                AddAsync(a, cancellationToken));
            }
            return true;
        }
        return false;
    }

    private async Task ImageUploadOrDelete(CardKeyDraft exCardToUpdate, string brandPrefix, string productPrefix, List<ImageInfo> listToUpload, List<ImageInfo> listToDelete, CancellationToken cancellationToken)
    {
        if (ListExtensions.HasData(listToUpload) || ListExtensions.HasData(listToDelete))
        {
            (List<ImageInfo> imagesUploaded, List<ImageInfo> imagesDeleted) = await azImageStorage.DeleteAndUploadImagesToCardId(exCardToUpdate.Id, listToUpload, listToDelete, cancellationToken);

            var brandImagesUploadedResult = imagesUploaded?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(brandPrefix))?.ToList();
            var productImagesUploadedResult = imagesUploaded?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(productPrefix))?.ToList();

            var brandImagesDeletedResult = imagesDeleted?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(brandPrefix))?.ToList();
            var productImagesDeletedResult = imagesDeleted?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(productPrefix))?.ToList();
            //delete first then upload entry,so replacement all handled properly
            //brand objects first
            if (brandImagesDeletedResult != null && ListExtensions.HasData(brandImagesDeletedResult))
                foreach (var imageInfo in brandImagesDeletedResult)
                {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(brandPrefix, string.Empty);
                    ImageInfo? matched = brandImagesDeletedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (matched != null && matched.DeletedStatus == true && imageInfo.ImageName != null)
                        exCardToUpdate.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exCardToUpdate, null);
                }
            if (brandImagesUploadedResult != null && ListExtensions.HasData(brandImagesUploadedResult))
                foreach (var imageInfo in brandImagesUploadedResult)
                {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(brandPrefix, string.Empty);
                    ImageInfo? matched = brandImagesUploadedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (matched != null && imageInfo.ImageName != null)
                        exCardToUpdate.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exCardToUpdate, matched?.Url);
                }
            //product objects
            if (productImagesDeletedResult != null && ListExtensions.HasData(productImagesDeletedResult))
                foreach (var imageInfo in productImagesDeletedResult)
                {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(productPrefix, string.Empty);
                    ImageInfo? matched = productImagesDeletedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (exCardToUpdate.CardDetail != null && matched != null && matched.DeletedStatus == true && imageInfo.ImageName != null)
                        exCardToUpdate.CardDetail.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exCardToUpdate.CardDetail, null);
                }
            if (productImagesUploadedResult != null && ListExtensions.HasData(productImagesUploadedResult))
                foreach (var imageInfo in productImagesUploadedResult)
                {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(productPrefix, string.Empty);
                    ImageInfo? matched = productImagesUploadedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (exCardToUpdate.CardDetail != null && matched != null && imageInfo.ImageName != null)
                        exCardToUpdate.CardDetail.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exCardToUpdate.CardDetail,
                        matched?.Url);
                }
        }
    }

    private void DetailsImagesUpdates(CreateUpdateCardCommand request, CardKeyDraft existingCard, CardKeyDraft exCardToUpdate, string cardDetailImagesPrefix, List<ImageInfo> listToUpload, List<ImageInfo> listToDelete)
    {
        if (CardDetailDto.IsNotImageUrlNorBase64(request.CardDetail) && CardDetail.IsNotImageUrls(existingCard.CardDetail))
        {
            //nothing just ignore,no difference
        }
        else if (CardDetailDto.IsNotImageUrlNorBase64(request.CardDetail) && !CardDetail.IsNotImageUrls(existingCard.CardDetail))
        {
            //means now all empty, existing has some data
            //on first screen CU component ,might not send detail page
            //but details page would send this
            //so if whole component didnt come then dont do anything,leave as it is by copying earlier to now
            exCardToUpdate.CardDetail = existingCard.CardDetail;
        }
        else if (request.CardDetail != null && !(CardDetailDto.IsNotImageUrlNorBase64(request.CardDetail) && CardDetail.IsNotImageUrls(existingCard.CardDetail)))
        {//todo also one more like both are equal then also skip
            //request.CardDetail??= new();//means adding new
            var (listToUploadProduct, listToDeleteProduct) = request.CardDetail.GetUploadAndDeleteImages(exCardToUpdate.CardDetail);
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
        }
    }

    private void MainImagesUpdate(CreateUpdateCardCommand request, CardKeyDraft existingCard, CardKeyDraft exCardToUpdate, string cardMainImagesPrefix, List<ImageInfo> listToUpload, List<ImageInfo> listToDelete)
    {
        if (CreateUpdateCardCommand.IsNotImageUrlNorBase64(request) && CreateUpdateCardCommand.IsNotImageUrls(existingCard))
        {
            //nothing just ignore,no difference
        }
        else if (CreateUpdateCardCommand.IsNotImageUrlNorBase64(request) && !CreateUpdateCardCommand.IsNotImageUrls(existingCard))
        {
            //means now all null, existing has some data
            //on first screen CU component ,might not send detail page
            //but details page would send this
            //so if whole component didnt come then dont do anything,leave as it is by copying earlier to now
            exCardToUpdate.Image1 = existingCard.Image1;
            exCardToUpdate.Image2 = existingCard.Image2;
        }
        else if (!(CreateUpdateCardCommand.IsNotImageUrlNorBase64(request) && CreateUpdateCardCommand.IsNotImageUrls(existingCard)))
        {//todo also one more like both are equal then also skip
            var (listToUploadBrand, listToDeleteBrand) = request.GetUploadAndDeleteImages(existingCard);
            if (ListExtensions.HasData(listToUploadBrand))
            {
                listToUploadBrand.ForEach(x => x.ImageName = cardMainImagesPrefix + x.ImageName ?? "");
                listToUpload.AddRange(listToUploadBrand);
            }
            if (ListExtensions.HasData(listToDeleteBrand))
            {
                listToDeleteBrand.ForEach(x => x.ImageName = cardMainImagesPrefix + x.ImageName ?? "");
                listToDelete.AddRange(listToDeleteBrand);
            }
        }
    }
}
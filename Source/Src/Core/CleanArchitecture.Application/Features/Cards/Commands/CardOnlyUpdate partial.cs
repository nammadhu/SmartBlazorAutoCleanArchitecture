using CleanArchitecture.Application;

namespace MyTown.Application.Features.Cards.Commands;

public partial class CU_CardCommandHandler
    {
    /*
     if (isverified=false ||(isverified=true && isforverified=true)) then all update in same table
    else if (isverified=true && isforverifeidcard=false) then on draft table
     */
    /* in order only
         * KeyInfoPlusDataChanged(images)
         * DataChanged
         * DetailChanged(images)
         * KeyInfoChanged(images)
         * ApprovalChangedByAdmin
         * ApproversChanged
         * IsOpenCloseChanged
         *
         * Approve/Reject is  separate endpoint or can club
         */

    private async Task<BaseResult<iCardDto>> UpdateCardOnly(CU_CardCommand updateCommand, CancellationToken cancellationToken)
        {
        /*
         Cases:
Create Card +Data +images
Update Card
Update Data
Update Detail
Update IsOpenClose
Update Approval
         */
        //UpdateCardOnly only for card main approvers info, NotFiniteNumberException for data or detail
        //for drafts, just restrict on ui itself,for weekly only 3 updates.in code no restriction
        //so cache will be updated always to make it quicker

        iCardDto? existingCardDto = null;
        (TownCardsDto? townCache, DateTime? cacheSetTime) = cachingServiceTown.Get<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(updateCommand.IdTown));
        if (townCache?.VerifiedCards != null && townCache.VerifiedCards.Exists(x => x.Id == updateCommand.Id))
            existingCardDto = townCache.VerifiedCards.FirstOrDefault(x => x.Id == updateCommand.Id);
        else if (townCache?.DraftCards != null && townCache.DraftCards.Exists(x => x.Id == updateCommand.Id))
            existingCardDto = townCache.DraftCards.FirstOrDefault(x => x.Id == updateCommand.Id);

        Card? existingCard;
        Card_DraftChanges? draft = null;
        if (existingCardDto == null)
            existingCard = await cardRepository.GetByIdAsync(updateCommand.Id, cancellationToken);
        else existingCard = mapper.Map<Card>(existingCardDto);
        if (existingCard == null)
            {
            return new Error(ErrorCode.NotFound, $"Cant update Non-Existing card with id {updateCommand.Id}", nameof(updateCommand.Id));
            }
        existingCard.OwnerDetail = null;//may be throwing error on update

        var cardToUpdate = existingCard.CloneBySerializing();
        if (cardToUpdate == null) throw new Exception("existingCard Clone Failed");

        List<ImageInfo> listToUpload = new();
        List<ImageInfo> listToDelete = new();
        bool pendingSubmit = false;
        bool isUpdateSuccess = false;
        /*
    if (isverified=false || isforverified=true) then all update in same table
   else if (isverified=true && isforverifeidcard=false) then on draft table
    */
        //mapper.Map<CreateUpdateCardCommand, Card_Draft>(updateCommand, cardToUpdate);//don't use this
        bool isToCardTable = false;
        bool isToDraftTable = false;

        if (updateCommand.IsForVerifiedCard == true || cardToUpdate.IsVerified != true ||
            !updateCommand.IsActiveSubscriber)
            {//either verified card or not an verified card ,no draft entry so  update here itself
            isToCardTable = true;
            isToDraftTable = false;
            CU_CardCommand.UpdateExistingDbEntity(cardToUpdate, updateCommand);
            if (!cardToUpdate.IsCardMainDataChanged(existingCard))
                return existingCardDto;
            cardToUpdate.IsVerified = updateCommand.IsForVerifiedCard;
            }
        else
            {//draft is only for already verified only
            isToCardTable = false;
            isToDraftTable = true;
            draft = await draftRepository.GetByIdAsync(updateCommand.Id, cancellationToken);
            if (draft == null)
                {
                draft = await draftRepository.AddAsync(mapper.Map<Card_DraftChanges>(updateCommand), cancellationToken);
                //await unitOfWork.SaveChangesAsync(cancellationToken);
                }
            else
                CU_CardCommand.UpdateExistingDbEntity(draft, updateCommand);
            }
        pendingSubmit = true;
        //main images(only),returns result either upload or delete
        bool modified = MainImagesUpdate(updateCommand, isToCardTable ? existingCard : draft!, ConstantsTown.CardMainImagesPrefix, listToUpload, listToDelete);
        if (modified && (ListExtensions.HasData(listToUpload) || ListExtensions.HasData(listToDelete)))
            {
            //if draft image has been used by verified,then delete should not be the case
            if (existingCard.IsVerified == true && ListExtensions.HasData(listToDelete))
                {
                List<ImageInfo> newListToDelete = [];// listToDelete.CloneBySerializing();
                listToDelete.ForEach(x =>
                {
                    if ((x.ImageName == ConstantsTown.CardMainImagesPrefix + nameof(existingCard.DraftChanges.Image1) && x.Url == existingCard.DraftChanges?.Image1)
                    || (x.ImageName == ConstantsTown.CardMainImagesPrefix + nameof(existingCard.DraftChanges.Image2) && x.Url == existingCard.DraftChanges?.Image2))
                        { }
                    else newListToDelete.Add(x);
                });
                listToDelete = newListToDelete;
                }

            await ImageUploadOrDelete(cardToUpdate, ConstantsTown.CardMainImagesPrefix, ConstantsTown.CardDetailImagesPrefix, listToUpload, listToDelete, cancellationToken);
            }
        //todo check if difference then only update otherwise skip... implement different or not

        if (isToCardTable)
            {
            if (existingCard.ApprovedPeerCardIds?.Count > 0 || updateCommand.SelectedApprovalCards?.Count > 0)
                {
                var changesPending = await AddUpdateApproverVerifiers(updateCommand.Id, existingCard.ApprovedPeerCardIds?.Select(x => new CardApproval() { IdCardOfApprover = x, IsVerified = true }).ToList(), updateCommand.SelectedApprovalCards, cancellationToken);
                if (!pendingSubmit) pendingSubmit = changesPending;
                }
            cardRepository.Update(cardToUpdate);
            }
        else if (isToDraftTable && draft != null)
            {
            if (draft.CardApprovals?.Count > 0 || updateCommand.SelectedApprovalCards?.Count > 0)
                {
                var changesPending = await AddUpdateApproverVerifiers(updateCommand.Id, draft.CardApprovals?.ToList(), updateCommand.SelectedApprovalCards, cancellationToken);
                if (changesPending)
                    {
                    if (!pendingSubmit) pendingSubmit = changesPending;
                    cardToUpdate.DraftChanges = draft;
                    }
                }
            draftRepository.Update(draft);
            }
        if (pendingSubmit)
            isUpdateSuccess = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (isUpdateSuccess)
            {//if requestIsVerified or existing not verified,go for  draft
             //not Updating cache for Existing verified & now draft then no cache update

            //currently only update cache for verified cards,not for drafts
            //currently refresh only to verified,if draft required then tweak here

            cardToUpdate.NullifyNavigatingObjectsTownCardType();
            var result = mapper.Map<iCardDto>(cardToUpdate);

            cachingServiceTown.AddOrUpdateCardInTown(updateCommand.IdTown, result, cardData: existingCardDto?.CardData, cardDetail: existingCardDto?.CardDetail, isVerified: result.IsVerified == true, townCardsModifiedTime: result.LastModified ?? DateTimeExtension.CurrentTime);
            return BaseResult<iCardDto>.OkNoClientCaching(result);
            }
        //no changes,so return original as it is
        return BaseResult<iCardDto>.OkNoClientCaching(mapper.Map<iCardDto>(existingCard) ?? new());
        }

    private async Task<bool> AddUpdateApproverVerifiers(int cardId, List<CardApproval>? existing, List<CardApproval>? newSelectedSet, CancellationToken cancellationToken)
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
            //newSelectedSet!.ForEach(async n => await townCardApprovalRepository.AddAsync(n, cancellationToken));
            //dont use foreach & async together as above instead use below
            for (int i = 0; i < newSelectedSet!.Count; i++)
                {
                var approvers = newSelectedSet[i].CloneBySerializing();
                if (approvers != null)
                    {
                    approvers.iCard = null;
                    approvers.ApproverCard = null;
                    await townCardApprovalRepository.AddAsync(approvers, cancellationToken);
                    }
                }

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
            //Detail images
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

    private bool MainImagesUpdate(CU_CardCommand request, _CardBase existingCard, string cardMainImagesPrefix, List<ImageInfo> listToUpload, List<ImageInfo> listToDelete)
    //Card_Draft exCardToUpdate,
        {
        if (CU_CardCommand.IsNotImageUrlNorBase64(request) && CU_CardCommand.IsNotImageUrls(existingCard))
            {
            //nothing just ignore,no difference
            return false;
            }
        else if (CU_CardCommand.IsNotImageUrlNorBase64(request) && !CU_CardCommand.IsNotImageUrls(existingCard))
            {
            //means now all null, existing has some data
            //on first screen CU component ,might not send detail page
            //but details page would send this
            //so if whole component didnt come then dont do anything,leave as it is by copying earlier to now
            return false;
            }
        else if (!(CU_CardCommand.IsNotImageUrlNorBase64(request) && CU_CardCommand.IsNotImageUrls(existingCard)))
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
            return true;
            }
        return false;
        }
    }

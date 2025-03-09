using CleanArchitecture.Application.Interfaces.UserInterfaces;

namespace MyTown.Application.Features.Cards.Commands;

public partial class CU_CardCommandHandler
    {
    /*
     Steps:New Card+Data Creating
CreateCommand=> Card_Draft(requestCard) & clone for later purpose
    toInsertCard is clone of requestCard(Card_Draft) with Data & detail=null
S1.Check if user exists on UserDetail table
S2.AddCard Draft(toInsertCard) SaveChanges() including Data & Detail entry creation
S3.if image exists then upload image & .Update Draft SaveChangesPending
s4.if verified then add same entry to draft table
if(isVerified)
{
S5.add to approval table with admin
S6.add to verified table
S7.add to town2Verified card
}
else
{}
SaveChanges() call

S8.add to cache of town
     */

    private async Task<BaseResult<iCardDto>> CreateCardWithData(CU_CardCommand createCommand,// UserDetailBase operatorOnGraphDb,
                                                                                             CancellationToken cancellationToken)
        {
        bool cardSavedResult = false;
        Card? newCard = null;
        bool isAdminOperator = false;
        if (createCommand != null)
            {
            //S1
            /* ad b2c
            if (operatorOnGraphDb?.Roles?.Count > 0 && operatorOnGraphDb.Roles.Any(x => CONSTANTS.ROLES.TownAdminWriters(createCommand.IdTown).Contains(x)))
                {
                //for admin dont do any user check,just proceed
                //but make sure user entry in system with graphdb roles

                //just directly adding admin to UserDetail table
                await userDetailRepository.AddUserRoles(operatorOnGraphDb.Id,
                      roles: operatorOnGraphDb.Roles,
                      operatorOnGraphDb.Id, existingUserInGraph: operatorOnGraphDb, cancellationToken);
                }
            */



            if (authenticatedUserService?.Roles?.Count > 0 && authenticatedUserService.Roles.Any(x => CONSTANTS.ROLES.TownAdminWriters(createCommand.IdTown).Contains(x)))

            //if (operatorOnGraphDb?.Roles?.Count > 0 && operatorOnGraphDb.Roles.Any(x => CONSTANTS.ROLES.TownAdminWriters(createCommand.IdTown).Contains(x)))
                {
                //for admin dont do any user check,just proceed
                //but make sure user entry in system with graphdb roles

                //just directly adding admin to UserDetail table
                await accountServices.AddRoleToUserAsync(authenticatedUserService.UserGuId,
                      roleNames: authenticatedUserService.Roles,
                      authenticatedUserService.UserGuId);
                //await userDetailRepository.AddUserRoles(operatorOnGraphDb.Id,
                //      roles: operatorOnGraphDb.Roles,
                //      operatorOnGraphDb.Id, existingUserInGraph: operatorOnGraphDb, cancellationToken);
                }
            else
                {
                createCommand.IsForVerifiedCard = false;
                isAdminOperator = false;


                if (authenticatedUserService.Roles.Count == 0 || authenticatedUserService.Roles.Contains(CONSTANTS.ROLES.Role_CardCreator) != true)
                    { //if already as creator then skip
                    await accountServices.AddRoleToUserAsync(authenticatedUserService.UserGuId, [CONSTANTS.ROLES.Role_CardCreator], authenticatedUserService.UserGuId);
                    }
                /* for adb2c
                UserDetailDto userOnSystemDto = await userDetailRepository.GetByIdIncludeCardsAsync(authenticatedUser.UserGuId, cancellationToken);
                UserDetail userOnSystem = mapper.Map<UserDetail>(userOnSystemDto);
                if (userOnSystem == null || userOnSystem == default)
                    {
                    UserDetail toInsertNewUser = mapper.Map<UserDetail>(operatorOnGraphDb);
                    toInsertNewUser.Roles ??= [];
                    toInsertNewUser.Roles.Add(CONSTANTS.ROLES.Role_CardCreator);
                    toInsertNewUser.CreatedBy = authenticatedUser.UserGuId;
                    userOnSystem = await userDetailRepository.AddAsync(toInsertNewUser, cancellationToken);
                    //lets insert as creator
                    //saveCHangesPending() will be with card insertion together itself
                    }
                else if (!userOnSystemDto.CanCreateNewCard(createCommand.IdTown))
                    {
                    return new BaseResult<iCardDto>()
                        {
                        Errors = [new Error(ErrorCode.FieldDataInvalid,
                    description: "Not Allowed to Create more than one Un-Verified Card")]
                        };
                    }

                if (userOnSystem.Roles?.Count == 0 || userOnSystem.Roles?.Contains(CONSTANTS.ROLES.Role_CardCreator) != true)
                    { //if already as creator then skip
                    userOnSystem.Roles ??= [];
                    userOnSystem.Roles.Add(CONSTANTS.ROLES.Role_CardCreator);
                    userDetailRepository.Update(userOnSystem);
                    //saveChangesPending() will be with card insertion together itself
                    }

                */
                }
            //add more check as if admin allow.
            //if townadmin then allow for current town only
            //other town again same rules only one un-verified card

            //here name can be duplicate allowed,no restriction

            //CreateCommand => Card_Draft(requestCard) & clone for later purpose
            //toInsertCard is clone of requestCard(Card_Draft) with Data & detail = null
            var requestCard = mapper.Map<Card>(createCommand);
            var toInsertCard = requestCard.CloneBySerializing() ?? throw new Exception("toInsert Clone() Failed");
            //toInsertCard.CardApprovals = null;

            if (createCommand.IsForVerifiedCard)
                {
                toInsertCard.IsVerified = true;
                toInsertCard.IsAdminVerified = true;
                }
            //S2.AddCard Draft(toInsertCard) SaveChanges() including Data &Detail entry creation
            newCard = await cardRepository.AddAsync(toInsertCard, cancellationToken);
            cardSavedResult = await unitOfWork.SaveChangesAsync(cancellationToken);

            if (newCard == null || !cardSavedResult || newCard.Id == 0)
                throw new Exception("iCard Insertion issue");

            createCommand.Id = newCard.Id;
            requestCard.Id = newCard.Id;

            //S3.if image exists then upload image
            if (CU_CardCommand.IsNewBase64Valid(createCommand))
                {
                var uploadedImageURLs = await azImageStorage.UploadImagesToCardId(createCommand.Id,
                            createCommand.ToUploadImages(), cancellationToken);
                if (uploadedImageURLs.HasData())
                    {
                    foreach (var imageInfo in uploadedImageURLs)
                        {
                        if (imageInfo?.ImageName != null)
                            newCard.GetType().GetProperty(imageInfo.ImageName)?.SetValue(newCard,
                                uploadedImageURLs.FirstOrDefault(x => x.ImageName == imageInfo.ImageName)?.Url
                                ); // Get uploaded URL
                        }
                    //S31.Update Draft
                    cardRepository.Update(newCard);
                    //savePending = true;
                    }
                }//else nothing to add for brand

            //S4.
            if (createCommand.IsForVerifiedCard)
                {//anybody add to approval table for reference
                //S5.add to approval table with admin
                //2 things,
                //if admin marked as verified then add to approval table & adminapproved=true
                //if peer approved/added, then add to approval table and peerapprovedids value as [3]
                //if self added then nothing

                if (createCommand.SelectedApprovalCards?.Count > 0)
                    {
                    createCommand.SelectedApprovalCards.RemoveAll(x => x.IdCardOfApprover == 0 && x.IdTown == 0);
                    foreach (var item in createCommand.SelectedApprovalCards)
                        {
                        item.IdTown = newCard.IdTown;
                        item.IdCard = newCard.Id;
                        item.IdCardOfApprover = null;//as by admin//todo if others then what???
                        item.UserId = requestCard.CreatedBy;
                        item.IsVerified = isAdminOperator;
                        }
                    await townCardApprovalRepository.AddRangeAsync(createCommand.SelectedApprovalCards, cancellationToken);
                    }

                //S6.add to verified table && S8 town2Verified

                //add entry in approver
                //add entry in verified card
                //add to Town2VerifiedCards
                //nothing more approval required

                //S7.add to town2Verified card ,this is for additional towns only
                //since only one town so no need to add to here as only one town
                //await additionalTownsOfVerifiedCardRepository.AddAsync(new AdditionalTownsOfVerifiedCard(newDraftCard.IdTown, newDraftCard.Id), cancellationToken);
                }

            bool cardUpdateSuccess = await unitOfWork.SaveChangesAsync(cancellationToken);

            //S8.add to cache of town
            if (cardSavedResult)//update cache if town exists in cache
                {
                //newDraftCard.CardData = dataAddedEntity;
                //newDraftCard.CardDetail = detailsAddedEntity;//currently not required as of default null always on create
                bool isForVerifiedCacheUpdate = false;
                iCardDto cacheCardToUpdate;
                DateTime townCardsModifiedTime = newCard.Created;

                newCard.NullifyNavigatingObjectsTownCardType();
                cacheCardToUpdate = mapper.Map<iCardDto>(newCard);
                //townModifiedTime = newDraftCard!.Created;//already set,so again not required

                cachingServiceTown.AddOrUpdateCardInTown(cacheCardToUpdate.IdTown, cacheCardToUpdate,
                    isVerified: isForVerifiedCacheUpdate, townCardsModifiedTime: townCardsModifiedTime);
                return BaseResult<iCardDto>.OkNoClientCaching(cacheCardToUpdate);
                }
            }
        return new Shared.Wrappers.Error(ErrorCode.NotHaveAnyChangeInData, "CreateCardWithData Failed as Null Request");
        }
    }

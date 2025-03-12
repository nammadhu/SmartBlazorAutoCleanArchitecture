using BASE;
using SHARED.Features.Cards.Commands;
using SHARED.Features.Cards.Queries;

namespace CleanArchitecture.WebApi.Controllers.v1;

[ApiVersion("1")]
public class TownCardController(IMediator mediator, IAuthenticatedUserService authenticatedUserService, ServerCachingTownCards serverCachingTownCards)
    : BaseApiController(mediator), ITownCardController
{//IIdentityRepository identityRepository,IHttpContextAccessor httpContextAccessor, ILogger<TownCardController> logger
    // this wont fetch town,instead only town cards. And drafts are only 100-verified
    //also fetches delta changes if lastServerFetchTime & isDeltaChangesOnly sent
    [HttpGet]//anonymous call for everyone
    public async Task<BaseResult<TownCardsDto>> GetCardsOfTown([FromQuery] GetCardsOfTownQuery model, CancellationToken cancellationToken = default)//this also delta
    {//most using ,town homepage by anonymous
        //for full result try to control otherwise all will missUse

        bool isAdmin = false;
        if (authenticatedUserService.IsAuthenticated &&
            authenticatedUserService.IsTownAdminWriters(model.IdTown))
            isAdmin = true;
        else
            model.RefreshByAdmin = false;

        var result = await Mediator.Send(model, cancellationToken).ConfigureAwait(false);

        if (result?.Data != null)
        {
            if (result.Data.VerifiedCards?.Count > 0)
            {
                if (model.DeltaChangesOnly && model.LastServerFetchTime != default)
                    result.Data.VerifiedCards = result.Data.VerifiedCards.ToList().Where(x => x.Created >= model.LastServerFetchTime || x.LastModified >= model.LastServerFetchTime).ToList();
                if (!isAdmin && result.Data.VerifiedCards?.Count > 0)
                    result.Data.VerifiedCards.ForEach(card => card.NullifyPrivateData());
                //every time  this filtration might be cumbersome,need to reThink of strategy
            }
            if (result.Data.DraftCards?.Count > 0)
            {
                if (model.DeltaChangesOnly && model.LastServerFetchTime != default)
                    result.Data.DraftCards = result.Data.DraftCards.ToList().Where(x => x.Created >= model.LastServerFetchTime || x.LastModified >= model.LastServerFetchTime).ToList();
                //currently not taking updates of drafts cards,but admins are exclusion.so taking here
                if (!isAdmin && result.Data.DraftCards?.Count > 0)
                    result.Data.DraftCards.ForEach(card => card.NullifyPrivateData());
                //every time  this filtration might be cumbersome,need to reThink of strategy
            }
        }
        return result;
    }

    [HttpGet]
    public async Task<BaseResult<TownCardsDto>> GetLatestCardsAllOrDelta(int IdTown, CancellationToken cancellationToken = default)
   => await GetCardsOfTown(new GetCardsOfTownQuery() { IdTown = IdTown }, cancellationToken);

    [HttpGet]
    public async Task<BaseResult<CardDto>> GetById([FromQuery] GetCardByIdQuery model, CancellationToken cancellationToken = default)
    {//both draft & approved here only,difference is inside Parameter isDraft
        var result = await Mediator.Send(model, cancellationToken);
        if (result.Data != null)
            result.Data.NullifyPrivateData();
        return result;
    }

    [HttpPost, Authorize]//lets allow everyone to create,later will block Blocked users
    public async Task<BaseResult<CardDto>> Create(CU_CardCommand createCommand, CancellationToken cancellationToken = default)
    {
        if (authenticatedUserService.IsAuthenticated && Guid.TryParse(authenticatedUserService.UserId, out Guid userGuId) && userGuId != Guid.Empty)
        {
            createCommand.ClientToServerDataExclusion();
            createCommand.Operator = userGuId;
            createCommand.IdOwner = userGuId;
            createCommand.CreatedBy = userGuId;

            if (createCommand.IsForVerifiedCard && !authenticatedUserService.IsTownAdminWriters(createCommand.IdTown)) //!User.IsTownAdminWriters(updateCommand.IdTown))
                createCommand.IsForVerifiedCard = false;

            //get prev icarddto from cache

            //once user created had to mark him with role of Creator
            //updateCommand.CreatedBy = UserIdExtract();
            //above is separately not required bcz ApplicationDbContext making default changes onCreate AuthUser id & onUpdate LastModifiedBy userGuId
            var result = await Mediator.Send(createCommand, cancellationToken);
            return result;
        }
        else throw new Exception("UserID Validation failed");
    }

    [HttpPut, Authorize(Roles = CONSTANTS.ROLES.Role_CardVerifiedOwner + "," + CONSTANTS.ROLES.Role_CardOwner + "," + CONSTANTS.ROLES.Role_CardCreator + "," + CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin)]
    public async Task<BaseResult<CardDto>> UpdateCard(CU_CardCommand updateCommand, CancellationToken cancellationToken = default)
    {
        //in case of cards transfer ,here might be required to add Creator or Owner role
        if (authenticatedUserService.IsAuthenticated && Guid.TryParse(authenticatedUserService.UserId, out Guid userId) && userId != Guid.Empty)
        {
            updateCommand.ClientToServerDataExclusion();
            updateCommand.LastModifiedBy = userId;
            updateCommand.Operator = userId;

            if (updateCommand.IsForVerifiedCard && !authenticatedUserService.IsTownAdminWriters(updateCommand.IdTown)) //!User.IsTownAdminWriters(updateCommand.IdTown))
                updateCommand.IsForVerifiedCard = false;

            //updateCommand.LastModifiedBy = UserIdExtract();
            var result = await Mediator.Send(updateCommand, cancellationToken);
            return result;
        }
        else throw new Exception("UserID Validation failed");
    }

    [HttpPut, Authorize(Roles = CONSTANTS.ROLES.Role_CardVerifiedOwner + "," + CONSTANTS.ROLES.Role_CardOwner + "," + CONSTANTS.ROLES.Role_CardCreator + "," + CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin)]
    public async Task<BaseResult<CardData>> UpdateCardData(CU_CardDataCommand model, CancellationToken cancellationToken = default)
    {
        if (model.IsVerified == true && !authenticatedUserService.IsTownAdminWriters(model.IdTown))
            model.IsVerified = null;
        //in case of cards transfer ,here might be required to add Creator or Owner role
        if (authenticatedUserService.IsAuthenticated)//redundant as Authorize but still to validate service working this is necessary
        {
            model.ClientToServerDataExclusion();
            //updateCommand.LastModifiedBy = UserIdExtract();
            var result = await Mediator.Send(model, cancellationToken);
            return result;
        }
        else throw new Exception("UserID Validation failed");
    }

    [HttpPut, Authorize(Roles = CONSTANTS.ROLES.Role_CardVerifiedOwner + "," + CONSTANTS.ROLES.Role_CardOwner + "," + CONSTANTS.ROLES.Role_CardCreator + "," + CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin)]
    public async Task<BaseResult<CardDetailDto>> UpdateCardDetail(CU_CardDetailCommand model, CancellationToken cancellationToken = default)
    {
        if (model.IsVerified == true && !authenticatedUserService.IsTownAdminWriters(model.IdTown))
            model.IsVerified = null;
        //in case of cards transfer ,here might be required to add Creator or Owner role
        if (authenticatedUserService.IsAuthenticated)//redundant as Authorize but still to validate service working this is necessary
        {
            model.ClientToServerDataExclusion();
            //updateCommand.LastModifiedBy = UserIdExtract();
            var result = await Mediator.Send(model, cancellationToken);
            return result;
        }
        else throw new Exception("UserID Validation failed");
    }

    [HttpPut, Authorize(Roles = CONSTANTS.ROLES.Role_CardVerifiedOwner + "," + CONSTANTS.ROLES.Role_CardOwner + "," + CONSTANTS.ROLES.Role_CardCreator + "," + CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin)]
    public async Task<BaseResult<bool?>> UpdateOpenClose(CardDetailOpenCloseUpdateCommand model, CancellationToken cancellationToken = default)
    {
        if (model.IsVerified == true && !authenticatedUserService.IsTownAdminWriters(model.IdTown))
            model.IsVerified = false;
        //in case of cards transfer ,here might be required to add Creator or Owner role
        if (authenticatedUserService.IsAuthenticated)//redundant as Authorize but still to validate service working this is necessary
        {
            //updateCommand.LastModifiedBy = UserIdExtract();
            var result = await Mediator.Send(model, cancellationToken);
            return result;
        }
        else throw new Exception("UserID Validation failed");
    }

    [HttpPut, Authorize(Roles = CONSTANTS.ROLES.Role_CardVerifiedOwner + "," + CONSTANTS.ROLES.Role_CardOwner + "," + CONSTANTS.ROLES.Role_CardCreator + "," + CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin + "," + CONSTANTS.ROLES.Role_TownAdmin + "," + CONSTANTS.ROLES.Role_TownReviewer)]
    public async Task<BaseResult<bool>> ApproveCard(ApproveCardCommand model, CancellationToken cancellationToken = default)
    {//if admin,else call normal peer approval
     //currently only admin allowed to approve
        return await Mediator.Send(model, cancellationToken);
    }

    /// <summary>
    /// Self Card_Drafts deletion or Admin can delete others
    /// </summary>
    /// <param name="cardId"></param>
    /// <returns></returns>
    [HttpDelete, Authorize]//todo had to move to different controller
    public async Task<BaseResult<bool>> Delete(DeleteCardCommand deleteCard, CancellationToken cancellationToken = default)
    {
        //wrt user mark number of cards he had created
        if (Guid.TryParse(authenticatedUserService.UserId, out Guid userId))
        {
            return await Mediator.Send(new DeleteCardCommand()
            {
                IdCard = deleteCard.IdCard,
                OperatorId = userId,
                IsAdmin = authenticatedUserService.IsInAnyOfRoles(CONSTANTS.ROLES.AdminWriters)
            }, cancellationToken);
        }
        return new BaseResult<bool> { Success = false };
    }

    [HttpPost, Authorize]
    public async Task<BaseResult<bool>> SetApproverCardOfDraftCard([FromBody] ApprovalCardSetRequestCommand request, CancellationToken cancellationToken = default)
    {//by user self,just makes entry in approval table
        try
        {
            var res = await Mediator.Send(request, cancellationToken);
            //Console.WriteLine(res.CardVerifiedItems.Count.ToString() + ":" + res.CardDrafts.Count().ToString());
            return res;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
    }

    [HttpGet]
    public async Task<(BaseResult<TownCardsDto>? cache, bool cacheStillValid)> GetTownCardsFromCache(int IdTown, CancellationToken cancellationToken = default)
    {
        (TownCardsDto cachedData, DateTime? cacheSetTime) = serverCachingTownCards.Get<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(IdTown));
        return (cachedData, true);
    }

    [HttpPost]//as a server side render only allowed to execute not as client to server call
    public Task UpdateTownCardsCache(int IdTown, TownCardsDto townCardsDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        //serverCachingTownCards.Set<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(IdTown), townCardsDto);
    }

    //private Guid UserIdExtract()
    //    {
    //    //this is separately not required bcz ApplicationDBcontext amking default changes onCreate Authuser id & onUpdate LastModifedBy userid
    //    var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //    if (Guid.TryParse(id, out Guid guid))
    //        {
    //        return guid;
    //        }
    //    throw new Exception("UserId Not Found");
    //    }

    #region OnTheFlyRoleRefreshLogicRemoved

    /*
    [HttpPut, Authorize]
    public async Task RefreshCreator(int idTown = 0)
    {
        if (!authenticatedUserService.IsInRole(CONSTANTS.ROLES.Role_CardCreator)
                   && (idTown > 0 ? !authenticatedUserService.IsTownAdminWriters(idTown) : true))
            await RoleAdditionToUserClaimsOnTheFly(CONSTANTS.ROLES.Role_CardCreator, HttpContext ?? httpContextAccessor.HttpContext);
    }
    */

    /* On CreateCard Refresh logic removed
           if (result != null && result.Success)
           {
               //in case of admin role,for approval it happens inside MovetoVerifiedCards method only
               //if (!User.IsInRole(CONSTANTS.ROLES.Role_CardCreator) && !User.IsTownAdminWriters(updateCommand.IdTown))
               if (!authenticatedUserService.IsInRole(CONSTANTS.ROLES.Role_CardCreator)
                   && !authenticatedUserService.IsTownAdminWriters(createCommand.IdTown))
               {
                   logger.LogWarning("card created,now adding creator role on graph db");
                   //add role of CardCreator on db
                   await identityRepository.AddUserRoles(authenticatedUserService.UserId, [CONSTANTS.ROLES.Role_CardCreator], cancellationToken);
                   await RoleAdditionToUserClaimsOnTheFly(CONSTANTS.ROLES.Role_CardCreator, HttpContext ?? httpContextAccessor.HttpContext);
                   //await CleanArchitecture.Infrastructure.Identity_Azure_AD_B2C.RoleClaimsService
                   //    .AddRoleToClaimsAsync(CONSTANTS.ROLES.Role_CardCreator,
                   //    HttpContext ?? httpContextAccessor.HttpContext);

                   //below is for aspnet identity
                   //logger.LogWarning("card created,now adding creator role to claims");
                   //await RoleAdditionToUserClaimsOnTheFly(CONSTANTS.ROLES.Role_CardCreator, HttpContext ?? httpContextAccessor.HttpContext);
                   //logger.LogWarning("card created,creator role added to all");
                   //todo CreatedCardCount+1... target also VerifiedCardCount not here
               }
               return result;
           }
           else
               return result.Errors;
           */

    /* On Update Card rrefresh logic removed
      if (result != null && result.Success)
    {
        //in case of admin role,for approval it happens inside MovetoVerifiedCards method only
        if (!authenticatedUserService.IsInRole(CONSTANTS.ROLES.Role_CardCreator)
            && !authenticatedUserService.IsTownAdminWriters(updateCommand.IdTown))
            await identityRepository.AddUserRoles(authenticatedUserService.UserId, [CONSTANTS.ROLES.Role_CardCreator], cancellationToken);////add role of CardCreator
        //todo CreatedCardCount+1... target also VerifiedCardCount not here
        return result;
    }
    else
        return result.Errors;
    */

    #endregion OnTheFlyRoleRefreshLogicRemoved
}

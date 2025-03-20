using SHARED.Features.Cards.Queries;

namespace CleanArchitecture.WebApi.Controllers.v1;

[ApiVersion("1")]
public class TownCardsController(IMediator mediator, IAuthenticatedUserService authenticatedUserService)//, ServerCachingTownCards serverCachingTownCards)
    : BaseApiController(mediator), ITownCardsController
    {//IIdentityRepository identityRepository,IHttpContextAccessor httpContextAccessor, ILogger<TownCardsController> logger
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
    }

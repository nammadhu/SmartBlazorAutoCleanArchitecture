using SHARED.Features.Cards.Queries;

namespace CleanArchitecture.WebApi.Controllers.v1;

[ApiVersion("1")]
public class MyCardsController(IMediator mediator, IAuthenticatedUserService authenticatedUserService)
    : BaseApiController(mediator), IMyCardsController
    {
    //user specific drafts and verified cards
    [HttpGet, Authorize]
    public async Task<BaseResult<UserDetailDto>> Get([FromQuery] GetCardsOfUserQuery request = default, CancellationToken cancellationToken = default)
        {//by user self
        try
            {
            if (authenticatedUserService.IsAdminWriters()) return null;
            //otherwise it can crash the system

            var res = await Mediator.Send(request ?? new(), cancellationToken);
            //Console.WriteLine(res.CardVerifiedItems.Count.ToString() + ":" + res.CardDrafts.Count().ToString());
            return res;
            }
        catch (Exception e)
            {
            Console.WriteLine(e.ToString());
            throw;
            }
        }

    [HttpGet, Authorize]
    public async Task<BaseResult<UserDetailDto>?> AddOrUpdateMyCards(CardDto iCardDto, CancellationToken cancellationToken = default)
        => await Get(cancellationToken: cancellationToken);

    [HttpGet, Authorize]
    public async Task<BaseResult<UserDetailDto>?> AddOrUpdateMyCardData(CardData cardData, CancellationToken cancellationToken = default) => await Get(cancellationToken: cancellationToken);

    [HttpGet, Authorize]
    public async Task<BaseResult<UserDetailDto>?> AddOrUpdateMyCardDetail(CardDetailDto cardDetail, CancellationToken cancellationToken = default) => await Get(cancellationToken: cancellationToken);

    [HttpGet, Authorize]
    public async Task<BaseResult<bool>> IsICanEditCard(int idCard, CancellationToken cancellationToken = default)//,int idTown=0)
        {
        var myCards = await Get(cancellationToken: cancellationToken);
        return myCards != null && myCards.Success && myCards.Data != null &&
            (myCards.Data.iCards?.Exists(x => x.Id == idCard) == true ||
            myCards.Data.iCards?.Exists(x => x.Id == idCard) == true);
        }

    [HttpDelete, Authorize]
    public async Task<BaseResult<bool>> DeleteMyAccountAndAllCardsCompletely(CancellationToken cancellationToken = default)
        {
        //wrt user mark number of cards he had created
        if (authenticatedUserService.IsAuthenticated)// if (Guid.TryParse(authenticatedUserService.UserId, out Guid operatorId))
            {
            return await Mediator.Send(new DeleteUserAndAllCardsCommand()
                {//here operator & user both himself
                TargetUserId = authenticatedUserService.UserGuId,
                OperatorId = authenticatedUserService.UserGuId,
                IsAdmin = authenticatedUserService.IsInAnyOfRoles(CONSTANTS.ROLES.AdminWriters)//User.IsInAnyOfRoles(CONSTANTS.ROLES.AdminWriters)
                }, cancellationToken);
            }
        return new BaseResult<bool> { Success = false };
        }
    }

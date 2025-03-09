namespace CleanArchitecture.Application.Features.Cards.Queries;

public class GetCardsOfUserQueryHandler(IAuthenticatedUserService authenticatedUserService, ICardRepository cardRepository) : IRequestHandler<GetCardsOfUserQuery, BaseResult<UserDetailDto>>
    {//IUserDetailRepository userDetailRepository,IUserDetailRepositoryAdB2c userDetailRepository
    public async Task<BaseResult<UserDetailDto>> Handle(GetCardsOfUserQuery request, CancellationToken cancellationToken)
        {
        if (authenticatedUserService.IsAuthenticated)
            {
            //we can move this to next layer as authenticatedUserService is accessible at all place
            request ??= new();
            request.UserId = authenticatedUserService.UserGuId;
            request.IsCardCreator = authenticatedUserService.IsInRole(CONSTANTS.ROLES.Role_CardCreator);
            request.IsCardOwner = authenticatedUserService.IsInRole(CONSTANTS.ROLES.Role_CardOwner);
            request.IsCardVerifiedOwner = authenticatedUserService.IsInRole(CONSTANTS.ROLES.Role_CardVerifiedOwner);
            //return await townCardRepo.GetUserCardsMoreDetails(request, cancellationToken);
            var userCards = await cardRepository.GetUserCards(authenticatedUserService.UserGuId, cancellationToken);

            return BaseResult<UserDetailDto>.Ok(new UserDetailDto()
                {
                //todo add roles all
                iCards = userCards
                }, ConstantsCachingServer.MyCards_MinCacheTimeSpan);
            }
        else throw new Exception("UserID Validation failed");
        }
    }

namespace CleanArchitecture.Application.Features.Cards.Queries
    {
    public class GetVerifiedCardsOfTypeInTownQueryHandler(ICard_DraftChangesRepository verifiedCardRepo) : IRequestHandler<GetVerifiedCardsOfTypeInTownQuery, BaseResult<List<iCardDto>>>
        {
        public async Task<BaseResult<List<iCardDto>>> Handle(GetVerifiedCardsOfTypeInTownQuery request, CancellationToken cancellationToken)
            {
            //return await verifiedCardRepo.GetVerifiedCardsOfTypeInTown(request, cancellationToken);
            return BaseResult<List<iCardDto>>.Ok(await verifiedCardRepo.GetVerifiedCardsOfTypeInTown(request, cancellationToken), ConstantsCachingServer.ApproverCards_MinCacheTimeSpan);
            }
        }
    }

namespace CleanArchitecture.Application.Features.Cards.Queries
    {
    public class GetVerifiedCardsOfTypeInTownQueryHandler(ICard_DraftChangesRepository verifiedCardRepo) : IRequestHandler<GetVerifiedCardsOfTypeInTownQuery, BaseResult<List<CardDto>>>
        {
        public async Task<BaseResult<List<CardDto>>> Handle(GetVerifiedCardsOfTypeInTownQuery request, CancellationToken cancellationToken)
            {
            //return await verifiedCardRepo.GetVerifiedCardsOfTypeInTown(request, cancellationToken);
            return BaseResult<List<CardDto>>.Ok(await verifiedCardRepo.GetVerifiedCardsOfTypeInTown(request, cancellationToken), ConstantsCachingServer.ApproverCards_MinCacheTimeSpan);
            }
        }
    }

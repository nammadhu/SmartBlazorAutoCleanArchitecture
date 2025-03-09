namespace CleanArchitecture.Application.Features.Cards.Queries
    {
    public class GetCardsAsAdminQueryHandler(ICardRepository cardRepo, ICard_DraftChangesRepository draftRepo) : IRequestHandler<GetCardsAsAdminQuery, BaseResult<List<iCardDto>>>
        {
        public async Task<BaseResult<List<iCardDto>>> Handle(GetCardsAsAdminQuery request, CancellationToken cancellationToken)
            {
            List<iCardDto> result = [];
            throw new NotImplementedException();
            //todo mostly not requried
            /*
            if (request.IsVerified == true)
                result = await cardRepo.GetTownVerifiedCardsAsync(request.IdTown, request.Name ?? string.Empty, cancellationToken);
            else if (request.IsDraft == true)
                result = await cardRepo.GetTownDraftCardsAsync(request.IdTown, request.Name ?? string.Empty, cancellationToken, request.IsVerified);
            */
            //return result;
            //return BaseResult<List<iCardDto>>.Ok(result, ConstantsCaching.UserCardsForAdmin_MinCacheTimeSpan);
            return new BaseResult<List<iCardDto>>() { Success = true, Data = result, MinCacheTimeSpan = ConstantsCachingServer.UserCardsForAdmin_MinCacheTimeSpan, MaxCacheTimeSpan = ConstantsCachingServer.UserCardsForAdmin_MaxCacheTimeSpan };
            }
        }
    }

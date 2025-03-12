namespace CleanArchitecture.Application.Features.Cards.Queries
    {
    public class GetCardsAsAdminPagedQueryHandler(ICardRepository cardRepo, ICard_DraftChangesRepository draftRepo) : IRequestHandler<GetCardsAsAdminPagedQuery, PagedResponse<CardDto>>
        {
        public async Task<PagedResponse<CardDto>> Handle(GetCardsAsAdminPagedQuery request, CancellationToken cancellationToken)
            {
            PagedResponse<CardDto> result = new PagedResponse<CardDto>();
            if (request.All)
                {
                request.PageNumber = 1;
                request.PageSize = int.MaxValue;
                }
            throw new NotImplementedException();
            //todo mostly not requried
            //if (request.IsVerified == true)
            //    result = await draftRepo.GetTownVerifiedCardsPagedListAsync(request.IdTown, request.PageNumber, request.PageSize, cancellationToken, request.Name ?? string.Empty);
            //else if (request.IsDraft == true)
            //    result = await cardRepo.GetTownDraftCardsPagedListAsync(request.IdTown, request.PageNumber, request.PageSize, request.Name ?? string.Empty, cancellationToken, request.IsVerified);

            return result;//new PagedResponse<iCardDto>(result);
                          //return PagedResponse<iCardDto>.Ok(result, ConstantsCaching.Card_MinCacheTimeSpan);
            }
        }
    }

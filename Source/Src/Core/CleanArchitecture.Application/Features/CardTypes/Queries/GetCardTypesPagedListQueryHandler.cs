namespace CleanArchitecture.Application.Features.CardTypes.Queries
    {
    public class GetCardTypesPagedListQueryHandler(ICardTypeRepository townCardTypeRepo, IMapper mapper) : IRequestHandler<GetCardTypesPagedListQuery, PagedResponse<CardTypeDto>>
        {
        public async Task<PagedResponse<CardTypeDto>> Handle(GetCardTypesPagedListQuery request, CancellationToken cancellationToken)
            {
            PaginationResponseDto<CardType> result;
            if (request.All)
                result = await townCardTypeRepo.GetPagedListAsync(1, int.MaxValue, request.Name ?? string.Empty, false, null, cancellationToken);
            else
                result = await townCardTypeRepo.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name ?? string.Empty, false, null, cancellationToken);
            var dto = result.Data.Select(x => mapper.Map<CardTypeDto>(x)).ToList();

            return new PaginationResponseDto<CardTypeDto>(dto, result.Count, request.PageNumber, request.PageSize);
            }
        }
    }

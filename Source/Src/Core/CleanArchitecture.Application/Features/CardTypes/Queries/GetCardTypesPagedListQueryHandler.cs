
namespace CleanArchitecture.Application.Features.CardTypes.Queries
{
    public class GetCardTypesPagedListQueryHandler(ICardTypeRepository townCardTypeRepo) : IRequestHandler<GetCardTypesPagedListQuery, PagedResponse<CardTypeDto>>
    {
        public async Task<PagedResponse<CardTypeDto>> Handle(GetCardTypesPagedListQuery request, CancellationToken cancellationToken)
        {
            PagedResponse<CardTypeDto> result;
            if (request.All)
                result = await townCardTypeRepo.GetPagedListAsync(1, int.MaxValue, request.Name ?? string.Empty, cancellationToken);
            else
                result = await townCardTypeRepo.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name ?? string.Empty, cancellationToken);

            return result;// new PagedResponse<CardTypeDto>(result);
        }
    }
}

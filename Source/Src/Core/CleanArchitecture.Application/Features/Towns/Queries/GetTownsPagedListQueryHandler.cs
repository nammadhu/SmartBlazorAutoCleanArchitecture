namespace CleanArchitecture.Application.Features.Towns.Queries
{
    public class GetTownsPagedListQueryHandler(ITownRepository townRepo) : IRequestHandler<GetTownsPagedListQuery, PagedResponse<TownDto>>
    {
        public async Task<PagedResponse<TownDto>> Handle(GetTownsPagedListQuery request, CancellationToken cancellationToken)
        {
            PagedResponse<TownDto> result;
            if (request.All)
                result = await townRepo.GetPagedListAsync(1, int.MaxValue, request.Name ?? string.Empty, cancellationToken);
            else
                result = await townRepo.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name ?? string.Empty, cancellationToken);

            return result;// new PagedResponse<TownDto>(result);
        }
    }
}

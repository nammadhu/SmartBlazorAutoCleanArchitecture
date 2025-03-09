namespace CleanArchitecture.Application.Features.Towns.Queries
    {
    public class GetTownsPagedListQueryHandler(ITownRepository townRepo, IMapper mapper) : IRequestHandler<GetTownsPagedListQuery, PagedResponse<TownDto>>
        {
        public async Task<PagedResponse<TownDto>> Handle(GetTownsPagedListQuery request, CancellationToken cancellationToken)
            {




            PagedResponse<Town> result;
            if (request.All)
                result = await townRepo.GetPagedListAsync(1, int.MaxValue, request.Name ?? string.Empty, false, null, cancellationToken);
            else
                result = await townRepo.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name ?? string.Empty, false, null, cancellationToken);
            var dto = result.Data.Select(x => mapper.Map<TownDto>(x)).ToList();

            return new PaginationResponseDto<TownDto>(dto, dto.Count, request.PageNumber, request.PageSize);
            }
        }
    }

using Shared.DTOs;
using Shared.Parameters;

namespace Shared.Features.Towns.Queries
    {
    public class GetTownsPagedListQuery : PaginationRequestParameter, IRequest<PagedResponse<TownDto>>
        {
        public bool All { get; set; }
        public string? Name { get; set; }
        }
    }

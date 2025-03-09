using Shared.DTOs;
using Shared.Features.Cards.Queries;

namespace Shared.Interfaces;

public interface ITownSelectedController
    {
    Task<BaseResult<TownDto>> GetCardsOfTown(GetCardsOfTownQuery model, CancellationToken cancellationToken = default);

    /* Instead will use signalR Mode
    Task<bool> CardUpdatesOfTownInBackground(int townId, CancellationToken cancellationToken = default);

    Task<PagedResponse<iCardDto>> GetTownCardsPagedQuery(GetCardsAsAdminPagedQuery model, CancellationToken cancellationToken = default);
    */
    /*  Task<BaseResult<List<iCardDto>>> GetTownCardsQueryAsAdmin(GetCardsAsAdminQuery model, CancellationToken cancellationToken = default);

      Task<BaseResult<List<iCardDto>>> GetVerifiedCardApproversOfTypeInTown(GetVerifiedCardsOfTypeInTownQuery request, CancellationToken cancellationToken = default);
      Task<BaseResult<List<iCardDto>>> GetVerifiedCardsOfTypeInTown(GetVerifiedCardsOfTypeInTownQuery request, CancellationToken cancellationToken = default);
      */
    }

using SHARED.DTOs;
using SHARED.Features.Cards.Commands;
using SHARED.Features.Cards.Queries;

namespace SHARED.Interfaces;

public interface ITownCardsController
    {
    Task<BaseResult<TownCardsDto>> GetCardsOfTown(GetCardsOfTownQuery model, CancellationToken cancellationToken = default);

    Task<BaseResult<TownCardsDto>> GetLatestCardsAllOrDelta(int IdTown, CancellationToken cancellationToken = default);
    }

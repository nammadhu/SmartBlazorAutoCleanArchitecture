using SHARED.DTOs;
using SHARED.Features.Towns.Commands;

namespace SHARED.Interfaces;

public interface ITownController
    {
    Task<BaseResult<TownDto>> Create(CU_TownCommand command, CancellationToken cancellationToken = default);

    Task<BaseResult<TownDto>> CreateUpdate(CU_TownCommand command, CancellationToken cancellationToken = default);

    Task<BaseResult> Delete(int id, CancellationToken cancellationToken = default);

    Task<BaseResult<List<TownDto>>?> GetAll(CancellationToken cancellationToken = default);

    Task<BaseResult<TownDto>> GetTownHeaderById(int id, CancellationToken cancellationToken = default);

    Task<BaseResult<List<TownDto>>> Search(string name, CancellationToken cancellationToken = default);

    Task<BaseResult<TownDto>> Update(CU_TownCommand command, CancellationToken cancellationToken = default);

    //Task<BaseResult<List<TownDto>>> GetDeltaOfTownsList(DateTime lastServerFetchTime, CancellationToken cancellationToken = default);

    //Task<BaseResult<List<TownDto>>?> DeltaLoadOfTownsChanges(CancellationToken cancellationToken = default);
    }

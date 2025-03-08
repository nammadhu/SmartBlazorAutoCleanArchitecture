using MyTown.SharedModels.Features.CardTypes.Commands;
using MyTown.SharedModels.Features.CardTypes.Queries;

namespace MyTown.SharedModels.Interfaces
{
    public interface ITownCardTypeController
    {
        Task<BaseResult<CardTypeDto>> Create(CU_CardTypeCommand model, CancellationToken cancellationToken = default);

        Task<BaseResult<CardTypeDto>> CreateUpdate(CU_CardTypeCommand model, CancellationToken cancellationToken = default);

        Task<BaseResult> Delete(int id, CancellationToken cancellationToken = default);

        Task<BaseResult<List<CardTypeDto>>> GetAll(CancellationToken cancellationToken = default);

        Task<BaseResult<CardTypeDto>> GetById(int id, CancellationToken cancellationToken = default);

        Task<PagedResponse<CardTypeDto>> GetPagedList(GetCardTypesPagedListQuery model, CancellationToken cancellationToken = default);

        Task<BaseResult<CardTypeDto>> Update(CU_CardTypeCommand model, CancellationToken cancellationToken = default);
    }
}
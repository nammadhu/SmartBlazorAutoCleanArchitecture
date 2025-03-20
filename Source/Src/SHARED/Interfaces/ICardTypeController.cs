using SHARED.DTOs;
using SHARED.Features.CardTypes.Commands;
using SHARED.Features.CardTypes.Queries;

namespace SHARED.Interfaces;

public interface ICardTypeController
    {
    Task<BaseResult<CardTypeDto>> Create(CU_CardTypeCommand model, CancellationToken cancellationToken = default);

    Task<BaseResult<CardTypeDto>> CreateUpdate(CU_CardTypeCommand model, CancellationToken cancellationToken = default);

    Task<BaseResult> Delete(int id, CancellationToken cancellationToken = default);

    Task<BaseResult<List<CardTypeDto>>> GetAll(CancellationToken cancellationToken = default);

    Task<BaseResult<CardTypeDto>> GetById(int id, CancellationToken cancellationToken = default);

    Task<PagedResponse<CardTypeDto>> GetPagedList(GetCardTypesPagedListQuery model, CancellationToken cancellationToken = default);

    Task<BaseResult<CardTypeDto>> Update(CU_CardTypeCommand model, CancellationToken cancellationToken = default);
    }


using Shared.DTOs;
using Shared.Features.Cards.Commands;
using Shared.Features.Cards.Queries;

namespace Shared.Interfaces;

public interface ITownCardController
    {
    Task<(BaseResult<TownCardsDto>? cache, bool cacheStillValid)> GetTownCardsFromCache(int IdTown, CancellationToken cancellationToken = default);

    Task UpdateTownCardsCache(int IdTown, TownCardsDto townCardsDto, CancellationToken cancellationToken = default);

    Task<BaseResult<TownCardsDto>> GetCardsOfTown(GetCardsOfTownQuery model, CancellationToken cancellationToken = default);

    Task<BaseResult<TownCardsDto>> GetLatestCardsAllOrDelta(int IdTown, CancellationToken cancellationToken = default);

    Task<BaseResult<CardDto>> GetById(GetCardByIdQuery model, CancellationToken cancellationToken = default);

    //Task RefreshCreator(int idTown = 0);//  #region OnTheFlyRoleRefreshLogicRemoved

    //should be called only from internal of SignalR, not outside in SignalR system
    Task<BaseResult<CardDto>> Create(CU_CardCommand model, CancellationToken cancellationToken = default);

    //should be called only from internal of SignalR, not outside in SignalR system
    Task<BaseResult<CardDto>> UpdateCard(CU_CardCommand model, CancellationToken cancellationToken = default);

    Task<BaseResult<CardData>> UpdateCardData(CU_CardDataCommand model, CancellationToken cancellationToken = default);

    Task<BaseResult<CardDetailDto>> UpdateCardDetail(CU_CardDetailCommand model, CancellationToken cancellationToken = default);

    Task<BaseResult<bool?>> UpdateOpenClose(CardDetailOpenCloseUpdateCommand model, CancellationToken cancellationToken = default);

    Task<BaseResult<bool>> ApproveCard(ApproveCardCommand model, CancellationToken cancellationToken = default);

    Task<BaseResult<bool>> Delete(DeleteCardCommand deleteCard, CancellationToken cancellationToken = default);

    Task<BaseResult<bool>> SetApproverCardOfDraftCard(ApprovalCardSetRequestCommand request, CancellationToken cancellationToken = default);
    }

using SHARED.DTOs;
using SHARED.Features.Cards.Commands;
using SHARED.Features.Cards.Queries;

namespace SHARED.Interfaces;

public interface ICardController
    {
    Task<BaseResult<bool>> ApproveCard(ApproveCardCommand model, CancellationToken cancellationToken = default);
    Task<BaseResult<CardDto>> Create(CU_CardCommand createCommand, CancellationToken cancellationToken = default);
    Task<BaseResult<bool>> Delete(DeleteCardCommand deleteCard, CancellationToken cancellationToken = default);
    Task<BaseResult<CardDto>> GetById(GetCardByIdQuery model, CancellationToken cancellationToken = default);
    Task<BaseResult<bool>> SetApproverCardOfDraftCard(ApprovalCardSetRequestCommand request, CancellationToken cancellationToken = default);
    Task<BaseResult<CardDto>> UpdateCard(CU_CardCommand updateCommand, CancellationToken cancellationToken = default);
    Task<BaseResult<CardData>> UpdateCardData(CU_CardDataCommand model, CancellationToken cancellationToken = default);
    Task<BaseResult<CardDetailDto>> UpdateCardDetail(CU_CardDetailCommand model, CancellationToken cancellationToken = default);
    Task<BaseResult<bool?>> UpdateOpenClose(CardDetailOpenCloseUpdateCommand model, CancellationToken cancellationToken = default);
    }

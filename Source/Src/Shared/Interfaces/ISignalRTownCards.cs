using Shared.DTOs;
using Shared.Features.Cards.Commands;
using Shared.Features.Cards.Queries;

namespace Shared.Interfaces;

public interface ISignalRTownCards
    {
    const string ReceiveBusinessCard = "ReceiveBusinessCard";

    event Action<CardDto> OnCardReceived;

    Task<bool> InitializeAsync(int idTown);//, CancellationToken cancellationToken = default);

    ValueTask DisposeAsync();

    Task JoinGroup(GetCardsOfTownQuery model);//, CancellationToken cancellationToken = default);

    Task LeaveGroup(int townId);//, CancellationToken cancellationToken = default);

    Task<BaseResult<CardDto>?> Create(CU_CardCommand model);//, CancellationToken cancellationToken = default);

    Task<BaseResult<CardDto>?> UpdateCard(CU_CardCommand model);//, CancellationToken cancellationToken = default);

    Task<BaseResult<CardData>?> UpdateCardData(CU_CardDataCommand model);

    Task<BaseResult<CardDetailDto>?> UpdateCardDetail(CU_CardDetailCommand model);

    Task<BaseResult<bool?>> UpdateOpenClose(CardDetailOpenCloseUpdateCommand model);

    Task Approval(int idTown, int idCard, bool? isApproved);//still not implemented
    }

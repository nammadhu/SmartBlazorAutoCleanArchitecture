using MyTown.SharedModels.Features.Cards.Queries;

namespace MyTown.SharedModels.Interfaces;

public interface IMyCardsController
{
    Task<BaseResult<bool>> DeleteMyAccountAndAllCardsCompletely(CancellationToken cancellationToken = default);

    Task<BaseResult<UserDetailDto>> Get(GetCardsOfUserQuery? request = null, CancellationToken cancellationToken = default);

    Task<BaseResult<bool>> IsICanEditCard(int idCard, CancellationToken cancellationToken = default);

    Task<BaseResult<UserDetailDto>?> AddOrUpdateMyCards(iCardDto iCardDto, CancellationToken cancellationToken = default);

    Task<BaseResult<UserDetailDto>?> AddOrUpdateMyCardData(CardData cardData, CancellationToken cancellationToken = default);

    Task<BaseResult<UserDetailDto>?> AddOrUpdateMyCardDetail(CardDetailDto cardDetail, CancellationToken cancellationToken = default);
}
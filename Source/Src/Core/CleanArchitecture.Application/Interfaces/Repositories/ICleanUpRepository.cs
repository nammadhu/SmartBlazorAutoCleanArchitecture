namespace CleanArchitecture.Application.Interfaces.Repositories
    {
    public interface ICleanUpRepository
        {
        Task<int> DeleteCardAndDependentDataCompletely(int cardId, Guid userId, CancellationToken cancellationToken, bool isAdmin = false);

        Task<(List<int> cardsToDelete, int cardDeletionCount, IdentityResult userDeletionResult)> DeleteUserAndAllUserCardsAndDataCompletely(Guid targetUserId, Guid operatorUserId, CancellationToken cancellationToken, bool isAdmin = false);

        Task<iCardDto> GetCardFromTrash(int cardId, CancellationToken cancellationToken);
        }
    }

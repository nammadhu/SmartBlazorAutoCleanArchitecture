namespace MyTown.SharedModels.Interfaces;

public interface IAdminController
{
    Task<BaseResult<bool>> DeleteUserAndAllCardsCompletely(Guid userId, CancellationToken cancellationToken = default);
}
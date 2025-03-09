namespace CleanArchitecture.Application.Interfaces.Repositories;

public interface IUserDetailRepository : IGenericRepository<UserDetail>
    {
    Task<UserDetailDto> GetByIdIncludeCardsAsync(Guid userId, CancellationToken cancellationToken);

    Task<IdentityResult> AddUserRoles(Guid userId, List<string> roles, Guid operatorId,
        UserDetailBase existingUserInGraph, CancellationToken cancellationToken);
    }

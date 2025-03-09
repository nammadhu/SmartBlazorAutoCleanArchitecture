using Shared.DTOs;

namespace Shared.Interfaces;

public interface IIdentityController
    {
    BaseResult<List<RoleDto>> Roles();

    Task<List<UserDetailDto>> SearchUsersByNameOrEmailInAuthSystem(GetAllUsersRequest model, CancellationToken cancellationToken = default);

    Task<BaseResult<bool>> AddUserToRoles(UserDetailDto model, CancellationToken cancellationToken = default);

    Task<BaseResult<bool>> RemoveUserRoles(UserDetailDto model, CancellationToken cancellationToken = default);
    }

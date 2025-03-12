using BASE.Common;

namespace CleanArchitecture.Application.Interfaces.Repositories;

public interface IIdentityRepositoryAdB2C
    {
    List<RoleDto> Roles { get; }

    Task<UserDetailBase> GetUserAsync(string userId, CancellationToken cancellationToken);

    Task<List<string>?> GetUserRolesAsync(string userId, CancellationToken cancellationToken);

    Task<List<UserDetailBase>?> SearchUsersByNameOrEmail(string? name, string? email, CancellationToken cancellationToken);

    Task<IdentityResult> AddUserRoles(string userId, List<string> roles, CancellationToken cancellationToken);

    Task<bool> CheckCurrentUserInRole(string role, CancellationToken cancellationToken);

    Task<bool> CheckUserInRole(string userId, string role, CancellationToken cancellationToken);

    Task<IdentityResult> DeleteLoggedInUserCompletely(Guid userId, Guid operatorId, CancellationToken cancellationToken);

    Task<IdentityResult> DeleteUserCompletely(Guid userId, Guid operatorId, CancellationToken cancellationToken);

    Task<bool> RemoveLoggedInUserRoles(List<string> rolesToRemove, CancellationToken cancellationToken);

    Task<bool> RemoveUserRoles(string userId, List<string> rolesToRemove, CancellationToken cancellationToken);

    /*    Task<User?> GetLoggedInUserAsync(CancellationToken cancellationToken);
        Task<PagedResponse<UserDto>> GetPagedUsers(GetAllUsersRequest model, CancellationToken cancellationToken);
        Task<User?> GetUserAsync(string userId, CancellationToken cancellationToken);
        Task<List<string>?> GetUserRolesAsync(string userId, CancellationToken cancellationToken);

        Task<User?> UpdateLoggedInUserDisplayNameAsync(string name, CancellationToken cancellationToken);
        Task<User?> UpdateUserDisplayNameAsync(string userId, string name, CancellationToken cancellationToken);
        //Task<User?> UpdateUserRolesCustomAttributeByOverWriting(string userId, List<string> roles, CancellationToken cancellationToken);
        */
    }

//moved to usermanager
//Task<List<(string RoleName, Guid roleId, int? TownId)>> GetUserTownSpecificRolesAsync(Guid userid);
//Task<List<(string RoleName, Guid roleId, int? TownId)>> GetUserTownSpecificRolesAsync(ApplicationUser user);

/* currently not using,as inbuilt utilising
Task<BaseResult<AuthenticationResponse>> GetJwtByCreateAccountOrFetchWithSocialAsync(string userName, string email, string name, string subject, string loginProvider = "Google");
Task<IList<string>> GetUserRoles(string UserId);

Task<GoogleJsonWebSignature.Payload> ValidateGoogleJwtToken(string googleJwtToken);
*/

// Task<BaseResult<AuthenticationResponse>> AuthenticateByJwtTokenOfGoogleType2(string authorizationHeader);
//old for local auth
/*
Task<BaseResult<string>> RegisterGostAccount();
Task<BaseResult> ChangePassword(ChangePasswordRequest model);
Task<BaseResult> ChangeUserName(ChangeUserNameRequest model);
Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request);
Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username);
*/

using CleanArchitecture.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using PublicCommon;
using SharedResponse;
using SharedResponse.DTOs;

namespace CleanArchitecture.Infrastructure.Identity_Azure_AD_B2C;

public class Identity_Azure_AD_B2CRepository(GraphService graphService) : IIdentityRepository
//, IAuthenticatedUserService authenticatedUser, ILogger<Identity_Azure_AD_B2CRepository> logger
{
    public List<RoleDto> Roles => MasterData.Roles;
    //public async Task<List<RoleDto>> AllRolesAsync(CancellationToken cancellation = default)
    //{
    //    await Task.Delay(0);
    //    return MasterData.Roles;
    //}

    // public async Task<UserDto?> GetLoggedInUserAsync(CancellationToken cancellationToken)
    //=> await graphService.GetLoggedInUserAsync(cancellationToken);

    public async Task<UserDetailBase?> GetUserAsync(string userId, CancellationToken cancellationToken)
     => await graphService.GetUserAsync(userId, cancellationToken);

    //this wont workout,as self authentication
    // public async Task<List<string>?> GetLoggedInUserRolesAsync(CancellationToken cancellationToken)
    //=> await graphService.GetLoggedInUserRolesAsync(cancellationToken);

    public async Task<List<string>?> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
  => await graphService.GetUserRolesAsync(userId, cancellationToken);

    public async Task<IdentityResult> AddUserRoles(string userId, List<string> roles, CancellationToken cancellationToken) =>
        await graphService.AddUserRolesCustomAttribute(userId, roles, cancellationToken) != null
            ? IdentityResult.Success : IdentityResult.Failed();

    public async Task<bool> CheckUserInRole(string userId, string role, CancellationToken cancellationToken)
    => (await graphService.GetUserRolesAsync(userId, cancellationToken))?.Contains(role) ?? false;

    public async Task<bool> CheckCurrentUserInRole(string role, CancellationToken cancellationToken)
    => (await graphService.GetLoggedInUserRolesAsync(cancellationToken))?.Contains(role) ?? false;

    public async Task<UserDetailBase?> UpdateUserDisplayNameAsync(string userId, string name, CancellationToken cancellationToken)
    => await graphService.UpdateUserDisplayNameAsync(userId, name, cancellationToken);

    public async Task<bool> UpdateUserRolesCustomAttributeByOverWriting(string userId, List<string> roles, CancellationToken cancellationToken)
     => await graphService.UpdateUserRolesCustomAttributeByOverWriting(userId, roles, cancellationToken);

    public async Task<bool> RemoveLoggedInUserRoles(List<string> rolesToRemove, CancellationToken cancellationToken)
     => await graphService.RemoveLoggedInUserRoles(rolesToRemove, cancellationToken);

    public async Task<bool> RemoveUserRoles(string userId, List<string> rolesToRemove, CancellationToken cancellationToken)
    => await graphService.RemoveUserRoles(userId, rolesToRemove, cancellationToken);

    public async Task<IdentityResult> DeleteLoggedInUserCompletely(Guid userId, Guid operatorId, CancellationToken cancellationToken)
   => (await graphService.DeleteUserCompletely(userId.ToString(), cancellationToken))
        ? IdentityResult.Success : IdentityResult.Failed();

    public async Task<IdentityResult> DeleteUserCompletely(Guid userId, Guid operatorId, CancellationToken cancellationToken)
    => (await graphService.DeleteUserCompletely(userId.ToString(), cancellationToken))
         ? IdentityResult.Success : IdentityResult.Failed();

    public async Task<List<UserDetailBase>?> SearchUsersByNameOrEmail(string? name, string? email, CancellationToken cancellationToken)
     => await graphService.SearchUsersByNameOrEmail(name, email, cancellationToken);

    //public async Task<PagedResponse<UserDto>> GetPagedUsers(GetAllUsersRequest model, CancellationToken cancellationToken)
    // => await graphService.SearchUsersByNameOrEmailOrRole(model.Name,model.Email,model.Role, cancellationToken);
}
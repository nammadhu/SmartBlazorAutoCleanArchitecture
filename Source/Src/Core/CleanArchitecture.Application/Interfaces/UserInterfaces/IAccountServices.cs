using CleanArchitecture.Domain.AspNetIdentity;
using SHARED.DTOs.Account.Requests;
using SHARED.DTOs.Account.Responses;

namespace CleanArchitecture.Application.Interfaces.UserInterfaces;

public interface IAccountServices
    {
    //Task<ApplicationUser> GetUserAsync(Guid userId);
    Task<IList<string>> GetUserRolesAsync(Guid userId);
    Task<bool> IsUserInRoleAsync(Guid userId, string roleName);
    // Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName);
    Task<IdentityResult> AddRoleToUserAsync(Guid userId, List<string> roleNames,Guid operatorId);
    Task<IdentityResult> DeleteUserCompletely(Guid userId);
    Task<IdentityResult> RemoveUserRoles(Guid userId, List<string> roleNames, Guid operatorId);
    Task<List<ApplicationUser>> SearchUsersByNameOrEmail(string searchTerm);


    Task<BaseResult<string>> RegisterGhostAccount();
    Task<BaseResult> ChangePassword(ChangePasswordRequest model);
    Task<BaseResult> ChangeUserName(ChangeUserNameRequest model);
    Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request);
    Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username);

    }

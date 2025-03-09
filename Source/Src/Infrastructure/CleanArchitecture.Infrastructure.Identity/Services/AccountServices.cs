using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.UserInterfaces;
using CleanArchitecture.Domain.AspNetIdentity;
using CleanArchitecture.Infrastructure.Identity.Contexts;
using CleanArchitecture.Infrastructure.Identity.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.DTOs;
using Shared.DTOs.Account.Requests;
using Shared.DTOs.Account.Responses;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Identity.Services;

public class AccountServices(UserManager<ApplicationUser> userManager, IAuthenticatedUserService authenticatedUser, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, JwtSettings jwtSettings, ITranslator translator
    ,IdentityContext dbContext) : IAccountServices
    {

    public async Task<ApplicationUser> GetUserAsync(Guid userId)
        => await userManager.FindByIdAsync(userId.ToString());

    public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
        try
            {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                {
                return new List<string>();
                }

            var roles = await userManager.GetRolesAsync(user);
            return roles;
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error getting user roles: {ex.Message}");
            return new List<string>();
            }
        }

    public async Task<bool> IsUserInRoleAsync(Guid userId, string roleName)
        {
        try
            {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                {
                return false;
                }

            return await userManager.IsInRoleAsync(user, roleName);
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error checking user role: {ex.Message}");
            return false;
            }
        }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName)
        {
        try
            {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
                {
                return new List<ApplicationUser>();
                }

            var users = await userManager.GetUsersInRoleAsync(roleName);
            return (IList<ApplicationUser>)users;
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error getting users in role: {ex.Message}");
            return new List<ApplicationUser>();
            }
        }

    public async Task<IdentityResult> AddRoleToUserAsync(Guid userId, List<string> roleNames, Guid operatorId)
        {
        //todo need to make operatorid to auditing
        try
            {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                }

            var existingRoles = await userManager.GetRolesAsync(user);

            var rolesToAdd = roleNames.Where(roleName => !existingRoles.Contains(roleName)).ToList();

            if (rolesToAdd.Count == 0)
                {
                return IdentityResult.Success; // All roles were already assigned.
                }

            var rolesExist = true;
            foreach (var roleName in rolesToAdd)
                {
                if (!await roleManager.RoleExistsAsync(roleName))
                    {
                    rolesExist = false;
                    break;
                    }
                }
            if (!rolesExist)
                {
                return IdentityResult.Failed(new IdentityError { Description = "One or more roles do not exist." });
                }

            var result = await userManager.AddToRolesAsync(user, rolesToAdd);
            return result;
            }
        catch (Exception ex)
            {
            // Log the exception
            Console.WriteLine($"Error adding role to user: {ex.Message}");
            return IdentityResult.Failed(new IdentityError { Description = "An error occurred while adding the role." });
            }
        }

    public async Task<IdentityResult> DeleteUserCompletely(Guid userId)
        {
        //todo need to add loggings
        try
            {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                }

            // Remove user roles
            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, userRoles);

            // Remove user logins
            var userLogins = await userManager.GetLoginsAsync(user);
            foreach (var login in userLogins)
                {
                await userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }

            // Remove user claims
            var userClaims = await userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
                {
                await userManager.RemoveClaimAsync(user, claim);
                }

            //Remove User Tokens
            var userTokens = await dbContext.UserTokens.Where(t => t.UserId == user.Id).ToListAsync();
            dbContext.UserTokens.RemoveRange(userTokens);
            await dbContext.SaveChangesAsync();

            // Remove the user
            var result = await userManager.DeleteAsync(user);

            return result;
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error removing user: {ex.Message}");
            return IdentityResult.Failed(new IdentityError { Description = "An error occurred while removing the user." });
            }
        }
    public async Task<List<ApplicationUser>> SearchUsersByNameOrEmail(string searchTerm)
        {
        try
            {
          return await userManager.Users
                .Where(u => u.UserName.Contains(searchTerm,StringComparison.OrdinalIgnoreCase) || u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error searching users: {ex.Message}");
            return [];
            }
        }

    public async Task<IdentityResult> RemoveUserRoles(Guid userId, List<string> roleNames, Guid operatorId)
        {
        try
            {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                }

            var existingRoles = await userManager.GetRolesAsync(user);

            var rolesToRemove = roleNames.Where(roleName => existingRoles.Contains(roleName)).ToList();

            if (rolesToRemove.Count == 0)
                {
                return IdentityResult.Success; // No roles needed to be removed.
                }

            var result = await userManager.RemoveFromRolesAsync(user, rolesToRemove);

            /*
            if (result.Succeeded)
                {
                // Audit log entries for each removed role
                foreach (var roleName in rolesToRemove)
                    {
                    var rl = (await roleManager.FindByNameAsync(roleName));
                    var applicationUserRole = await dbContext.UserRoles
                        .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == rl.Id);

                    if (applicationUserRole != null)
                        {
                        applicationUserRole.OperatorId = operatorId;
                        applicationUserRole.Timestamp = DateTime.UtcNow;
                        dbContext.UserRoles.Remove(applicationUserRole);
                        await dbContext.SaveChangesAsync();
                        }
                    }
                return result;
                }
            else
                {
                return result;
                }
            */
            return result;
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error removing user roles: {ex.Message}");
            return IdentityResult.Failed(new IdentityError { Description = "An error occurred while removing the roles." });
            }
        }




    public async Task<BaseResult> ChangePassword(ChangePasswordRequest model)
        {
        var user = await userManager.FindByIdAsync(authenticatedUser.UserId);

        var token = await userManager.GeneratePasswordResetTokenAsync(user!);
        var identityResult = await userManager.ResetPasswordAsync(user, token, model.Password);

        if (identityResult.Succeeded)
            return BaseResult.Ok();

        return identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)).ToList();
        }

    public async Task<BaseResult> ChangeUserName(ChangeUserNameRequest model)
        {
        var user = await userManager.FindByIdAsync(authenticatedUser.UserId);

        user.UserName = model.UserName;

        var identityResult = await userManager.UpdateAsync(user);

        if (identityResult.Succeeded)
            return BaseResult.Ok();

        return identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)).ToList();
        }

    public async Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
        {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null)
            {
            return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.AccountMessages.Account_NotFound_with_UserName(request.UserName)), nameof(request.UserName));
            }

        var signInResult = await signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
        if (!signInResult.Succeeded)
            {
            return new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.AccountMessages.Invalid_password()), nameof(request.Password));
            }

        return await GetAuthenticationResponse(user);
        }

    public async Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username)
        {
        var user = await userManager.FindByNameAsync(username);
        if (user == null)
            {
            return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.AccountMessages.Account_NotFound_with_UserName(username)), nameof(username));
            }

        return await GetAuthenticationResponse(user);
        }

    public async Task<BaseResult<string>> RegisterGhostAccount()
        {
        var user = new ApplicationUser()
            {
            UserName = GenerateRandomString(7)
            };

        var identityResult = await userManager.CreateAsync(user);

        if (identityResult.Succeeded)
            return user.UserName;

        return identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)).ToList();

        string GenerateRandomString(int length)
            {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }

    private async Task<AuthenticationResponse> GetAuthenticationResponse(ApplicationUser user)
        {
        await userManager.UpdateSecurityStampAsync(user);

        var jwToken = await GenerateJwtToken();

        var rolesList = await userManager.GetRolesAsync(user);

        return new AuthenticationResponse()
            {
            Id = user.Id.ToString(),
            JwToken = new JwtSecurityTokenHandler().WriteToken(jwToken),
            Email = user.Email,
            UserName = user.UserName,
            Roles = rolesList,
            IsVerified = user.EmailConfirmed,
            };

        async Task<JwtSecurityToken> GenerateJwtToken()
            {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: (await signInManager.ClaimsFactory.CreateAsync(user)).Claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            }
        }
    }

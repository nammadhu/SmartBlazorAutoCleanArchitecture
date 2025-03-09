using Shared.DTOs.Account.Requests;
using Shared.DTOs.Account.Responses;

namespace CleanArchitecture.Application.Interfaces.UserInterfaces;

public interface IAccountServices
    {
    Task<BaseResult<string>> RegisterGhostAccount();
    Task<BaseResult> ChangePassword(ChangePasswordRequest model);
    Task<BaseResult> ChangeUserName(ChangeUserNameRequest model);
    Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request);
    Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username);

    }

using Shared.DTOs.Account.Responses;

namespace CleanArchitecture.Application.Interfaces.UserInterfaces;

public interface IGetUserServices
    {
    Task<PagedResponse<UserDto>> GetPagedUsers(GetAllUsersRequest model);
    }

using Shared.DTOs;
using Shared.DTOs.Account.Responses;
using Shared.Wrappers;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.UserInterfaces;

public interface IGetUserServices
{
    Task<PagedResponse<UserDto>> GetPagedUsers(GetAllUsersRequest model);
}

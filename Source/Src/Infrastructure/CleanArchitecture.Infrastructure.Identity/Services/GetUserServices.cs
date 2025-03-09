using CleanArchitecture.Application.Interfaces.UserInterfaces;
using CleanArchitecture.Infrastructure.Identity.Contexts;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.DTOs.Account.Responses;
using Shared.Wrappers;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Identity.Services;

public class GetUserServices(IdentityContext identityContext) : IGetUserServices
    {
    public async Task<PagedResponse<UserDto>> GetPagedUsers(GetAllUsersRequest model)
        {
        var skip = (model.PageNumber - 1) * model.PageSize;

        var users = identityContext.Users
            .Select(p => new UserDto()
                {
                Name = p.Name,
                Email = p.Email,
                UserName = p.UserName,
                PhoneNumber = p.PhoneNumber,
                Id = p.Id,
                Created = p.Created,
                });

        if (!string.IsNullOrEmpty(model.Name))
            {
            users = users.Where(p => p.Name.Contains(model.Name));
            }

        return new PaginationResponseDto<UserDto>(
            await users.Skip(skip).Take(model.PageSize).ToListAsync(),
            await users.CountAsync(),
            model.PageNumber,
            model.PageSize);

        }
    }

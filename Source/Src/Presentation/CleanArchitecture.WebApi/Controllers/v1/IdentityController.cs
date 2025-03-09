namespace CleanArchitecture.WebApi.Controllers.v1;

[ApiVersion("1")]
public class IdentityController(IIdentityRepository identityRepository, IMediator mediator,
     IAuthenticatedUserService authenticatedUserService, IMapper mapper) : BaseApiController(mediator), IIdentityController
//IUserDetailRepository userDetailRepository,
//IMediator mediator, IAuthenticatedUserService authenticatedUserService) : BaseApiController(mediator)
{
    //todo must authorize for production
    [HttpGet, Authorize(Roles = CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin)]
    public async Task<List<UserDetailDto>> SearchUsersByNameOrEmailInAuthSystem([FromQuery] GetAllUsersRequest model, CancellationToken cancellationToken = default)
    {//make this as only search of graph db
        var userInGraph = (await identityRepository.SearchUsersByNameOrEmail(model.Name, model.Email, cancellationToken));
        // model.Roles.First().TID??0),
        return mapper.Map<List<UserDetailDto>>(userInGraph);
    }

    [HttpGet, Authorize(Roles = CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin)]
    public Task<List<UserDetailDto>> SearchUsersByNameOrEmailInDatabase([FromQuery] GetAllUsersRequest model, CancellationToken cancellationToken = default)
    {//make this as only search of graph db
        throw new NotImplementedException();
    }

    //[HttpGet, Authorize(Roles = CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin)]
    //public async Task<List<UserDetailDto>> SearchUsersByNameOrEmailInAll([FromQuery] GetAllUsersRequest model, CancellationToken cancellationToken = default)
    //{//make this as only search of graph db
    //    var userInGraph = (await identityRepository.SearchUsersByNameOrEmail(model.Name, model.Email, cancellationToken));
    //    if (userInGraph != null)
    //    {
    //        userDetailRepository.GetByIdAsync(userInGraph.)
    //        // model.Roles.First().TID??0),
    //        return mapper.Map<List<UserDetailDto>>(userInGraph);
    //    }
    //}

    [HttpGet]
    public BaseResult<List<RoleDto>> Roles()//CancellationToken cancellationToken = default)
    {
        //var t1=identityRepository.Roles;
        return MasterData.Roles;
    }

    [HttpPost, Authorize(Roles = CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin)]
    public async Task<BaseResult<bool>> AddUserToRoles(UserDetailDto model, CancellationToken cancellationToken = default)
    {
        if (authenticatedUserService.IsAuthenticated && Guid.TryParse(authenticatedUserService.UserId, out Guid userGuId) && userGuId != Guid.Empty)
        {
            //in aspnet identity:currently multiple roles with townid is not working,instead does for first tole only
            //townid=0 works fine for multiple roles... but currently UI handling only for one role

            if (userGuId == model.Id) return false;//should not be allowed for self assigning

            //todo must, should not allow other to add any role above him

            var townRoles = model.RoleDtos.Where(x => x.RoleName.StartsWith("Towns")).ToList();
            if (townRoles?.Count() > 0)
            {
                model.RoleDtos.RemoveAll(x => townRoles.Select(c => c.RoleName).Contains(x.RoleName));
                townRoles.RemoveAll(x => x.TID == 0);
                if (townRoles?.Count > 0)
                {
                    foreach (var townRole in townRoles)
                    {
                        townRole.RoleName = townRole.RoleName + "_" + townRole.TID;
                    }
                    model.RoleDtos.AddRange(townRoles);
                }
            }

            return (await identityRepository.AddUserRoles(model.Id.ToString(), model.RoleDtos.Select(x => x.RoleName).ToList(), cancellationToken)).Succeeded;// model.Roles.First().TID??0),
        }
        return false;
    }

    [HttpPost, Authorize(Roles = CONSTANTS.ROLES.Role_Admin + "," + CONSTANTS.ROLES.Role_InternalAdmin)]
    public async Task<BaseResult<bool>> RemoveUserRoles(UserDetailDto model, CancellationToken cancellationToken = default)
    {
        //currently multiple roles with townid is not working,instead does for first tole only
        //townid=0 works fine for multiple roles... but currently UI handling only for one role
        return (await identityRepository.RemoveUserRoles(model.Id.ToString(), model.RoleDtos.Select(x => x.RoleName).ToList(), cancellationToken));//model.Roles.First().TID ?? 0
    }
}

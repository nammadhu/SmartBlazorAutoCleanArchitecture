using PublicCommon;
using System.Security.Claims;

namespace Shared;

//keep this place for town specific settings only
//common inside publicCommon
public static class UserClaimsExtensionsTown
    {
    public static bool IsAdminWriters(this ClaimsPrincipal user)
        => user.IsInAnyOfRoles(CONSTANTS.ROLES.AdminWriters);

    public static bool IsTownAdminWriters(this ClaimsPrincipal user, int townId = 0)
        => user.IsInAnyOfRoles(CONSTANTS.ROLES.TownAdminWriters(townId));
    }

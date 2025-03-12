using BASE;
using System.Security.Claims;

namespace SHARED;

//keep this place for town specific settings only
//common inside BASE
public static class UserClaimsExtensionsTown
    {
    public static bool IsAdminWriters(this ClaimsPrincipal user)
        => user.IsInAnyOfRoles(CONSTANTS.ROLES.AdminWriters);

    public static bool IsTownAdminWriters(this ClaimsPrincipal user, int townId = 0)
        => user.IsInAnyOfRoles(CONSTANTS.ROLES.TownAdminWriters(townId));
    }

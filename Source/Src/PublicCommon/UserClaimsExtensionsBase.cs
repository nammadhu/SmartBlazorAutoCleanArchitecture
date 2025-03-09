using System.Security.Claims;

namespace PublicCommon;

//keep this place for common settings only
//town or app specific inside MyTown.SharedModels
public static class UserClaimsExtensionsBase
    {
    public static bool IsAdmin(this ClaimsPrincipal user)
        => user != null && user.IsInRole(CONSTANTS.ROLES.Role_Admin);// || user.IsInRole(CONSTANTS.ROLES.Role_InternalAdmin);

    public static string? Id(this ClaimsPrincipal user)
            => user != null ? user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value : default;

    public static Guid? GuId(this ClaimsPrincipal user)
        => Guid.TryParse(Id(user), out Guid result) ? (result != Guid.Empty ? result : null) : null;

    //ideally above should throw exception,as valid userId never go null //todo should

    public static string? Email(this ClaimsPrincipal user)
            => user != null ? user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value : default;

    public static List<string> Roles(this ClaimsPrincipal user)
            => user != null ? user.FindAll(c => c.Type == ClaimTypes.Role).Select(x => x.Value).ToList() : new();

    //public static bool IsInRole(this ClaimsPrincipal user, string role)
    //        => user != null && user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role);

    public static bool IsInAnyOfRoles(this ClaimsPrincipal user, params string[] inputRoles)
       => Roles(user).Any(x => inputRoles.Contains(x));

    public static bool IsInAnyOfRoles(this ClaimsPrincipal user, List<string> inputRoles)
        => user.IsInAnyOfRoles(inputRoles.ToArray());
    }
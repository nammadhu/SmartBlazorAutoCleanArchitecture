using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CleanArchitecture.Infrastructure.Identity_Azure_AD_B2C;

public class RoleClaimsService
{
    public static async Task AddRoleToClaimsAsync(string roleName, HttpContext context)
    {
        if (context?.User?.Identity?.IsAuthenticated != true)
            return;

        var principal = context.User;
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null) return;

        if (!identity.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == roleName))
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, roleName));

            var newPrincipal = new ClaimsPrincipal(identity);

            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            Console.WriteLine($"Roles after adding: {string.Join(", ", roles)}");

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, newPrincipal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(29)
            });
        }
    }
}
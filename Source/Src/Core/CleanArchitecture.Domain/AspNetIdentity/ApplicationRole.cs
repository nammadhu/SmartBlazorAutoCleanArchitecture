using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.AspNetIdentity;

public class ApplicationRole(string name) : IdentityRole<Guid>(name)
    {
    //ApplicationRole & UserRole is with Aspnet Identity System
    //AzureAd_B2C uses RoleDto of sharedResponse
    // public virtual ICollection<UserRole>? UserRoles { get; set; }
    }

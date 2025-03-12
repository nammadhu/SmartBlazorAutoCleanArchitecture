using BASE;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.AspNetIdentity;

public class ApplicationUser : IdentityUser<Guid>
{
    //ApplicationUser is with Aspnet Identity System
    //AzureAd_B2C uses UserInfo of sharedResponse
    public ApplicationUser()
    {
        Name ??= UserName;
    }

    public string? Name { get; set; }
    public DateTime Created { get; set; } = DateTimeExtension.CurrentTime;
    //public DateTime LastModified { get; set; }

    //public virtual ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
   // public virtual ICollection<UserRole>? UserRoles { get; set; }

    //[NotMapped]
    //public List<string>? Roles { get; set; }
}

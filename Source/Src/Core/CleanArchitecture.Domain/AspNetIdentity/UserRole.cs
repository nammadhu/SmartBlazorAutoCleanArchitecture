using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.AspNetIdentity;

public class UserRole : IdentityUserRole<Guid>
{
    //ApplicationRole & UserRole is with Aspnet Identity System
    //AzureAd_B2C uses RoleDto of sharedResponse
    public int? TId { get; set; }
    public virtual ApplicationUser? User { get; set; }
    public virtual ApplicationRole? Role { get; set; }

    public DateTime Created { get; set; } = PublicCommon.DateTimeExtension.CurrentTime;
    public DateTime LastModified { get; set; } = PublicCommon.DateTimeExtension.CurrentTime;

    //fk of role table //todo later
    //public ApplicationRole? Role { get; set; }
    //public string? TenantId { get; set; }//useful for MyTown, TownAdmin or reviewer role value will be like townid as "5",clientside had to parse
}

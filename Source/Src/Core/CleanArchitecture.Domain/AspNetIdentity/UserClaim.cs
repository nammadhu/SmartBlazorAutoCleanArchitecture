using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain
{
    //not using currently
    public class UserClaim : IdentityUserClaim<Guid>
    {
        public string? TenantId { get; set; }//useful for MyTown, TownAdmin or reviewer role value will be like townId as "5",clientSide had to parse
    }
}
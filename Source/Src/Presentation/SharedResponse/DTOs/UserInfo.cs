using PublicCommon;
using PublicCommon.Common;
using SharedResponse.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedResponse.DTOs;
//currently this is not using instead using UserDetail

//moved from BlazorWebAppAuto.Client
//add values at PersistingRevalidatingAuthenticationStateProvider & access at clientside by AuthenticationStateProviders
// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.

//this is combined of UserInfo + UserDto
public class UserInfo //: AuditableBaseEntity<Guid>
{
    //ApplicationUser is with Aspnet Identity System
    //AzureAd_B2C uses UserInfo of sharedResponse

    public Guid Id { get; set; }//already in base
    //public string UserName { get; set; } = default!;//mostly not useful so removed
    public string Name { get; set; } = default!;//displayname

    [EmailAddress]
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }

    [NotMapped]
    public List<RoleDto> RoleDtos { get; set; } = [];

    public List<string>? Roles { get; set; }//required to pass to clientSide
    public List<int>? CardIds { get; set; }
    public bool IsLikeCommentSubscribed { get; set; }
}
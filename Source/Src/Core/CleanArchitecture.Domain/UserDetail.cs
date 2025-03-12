using BASE.Common;

namespace CleanArchitecture.Domain;

//moved from BlazorWebAppAuto.Client
//add values at PersistingRevalidatingAuthenticationStateProvider & access at clientside by AuthenticationStateProviders
// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.

//this is combined of UserInfo + UserDto
public class UserDetail : AuditableBaseEntity<Guid>
    {
    //ApplicationUser is with Aspnet Identity System
    //AzureAd_B2C uses UserInfo of sharedResponse
    [Key]
    public override Guid Id { get; set; }//already in base

    //public string UserName { get; set; } = default!;//mostly not useful so removed
    public string Name { get; set; } = default!;//displayname

    [EmailAddress]
    public string Email { get; set; } = default!;

    public string? PhoneNumber { get; set; }

    public List<string>? Roles { get; set; }//required to pass to clientSide

    public bool IsLikeCommentSubscribed { get; set; }

    public virtual List<Card>? iCards { get; set; }
    //public virtual List<Card_DraftChanges>? CardsVerified { get; set; }
    //public virtual List<CardApproval>? CardApprovals { get; set; }
    }
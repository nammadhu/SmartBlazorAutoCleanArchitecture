using System.ComponentModel.DataAnnotations;

namespace SharedResponse.DTOs;

//moved from BlazorWebAppAuto.Client
//add values at PersistingRevalidatingAuthenticationStateProvider & access at clientside by AuthenticationStateProviders
// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public class UserInfo
{
    public UserInfo()
    {
    }

    public UserInfo(string name, string email, List<string> roles)
    {
        DisplayName = name;
        Email = email;
        Roles = roles;
    }

    [Key]
    public Guid Id { get; set; }

    //public string? DisplayName { get; set; }

    public string DisplayName { get; set; } = default!;

    //public string? Surname { get; set; }

    [EmailAddress]
    public string Email { get; set; } = default!;

    // Add properties based on the claims or details you want the user to have in the WebAssembly pages and components.

    public List<string>? Roles { get; set; }//required to pass to clientSide

    public List<string>? CardIds { get; set; }
    public bool IsLikeCommentSubscribed { get; set; }
}
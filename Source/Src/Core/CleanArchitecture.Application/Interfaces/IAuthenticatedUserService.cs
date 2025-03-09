using System.Security.Claims;

namespace CleanArchitecture.Application.Interfaces;

public interface IAuthenticatedUserService
    {
    string UserId { get; }
    string UserName { get; }

    //Below all are additional to saman
    public ClaimsPrincipal User { get; }
    bool IsAuthenticated { get; }

    Guid UserGuId { get; }
    string Name { get; }
    string Email { get; }

    public List<string> Roles { get; }
    bool IsInRole(string role);

    bool IsInAnyOfRoles(string[] roles);

    bool IsInAnyOfRoles(List<string> roles);

    public bool IsAdminWriters();

    public bool IsTownAdminWriters(int townId = 0);
    }

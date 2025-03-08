using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CleanArchitecture.Application.Interfaces;

public interface IAuthenticatedUserService
{
    public ClaimsPrincipal User { get; }
    bool IsAuthenticated { get; }
    string UserId { get; }
    Guid UserGuId { get; }
    string Name { get; }
    string Email { get; }
    string UserName { get; }

    bool IsInRole(string role);

    bool IsInAnyOfRoles(string[] roles);

    bool IsInAnyOfRoles(List<string> roles);

    public bool IsAdminWriters();

    public bool IsTownAdminWriters(int townId = 0);
}
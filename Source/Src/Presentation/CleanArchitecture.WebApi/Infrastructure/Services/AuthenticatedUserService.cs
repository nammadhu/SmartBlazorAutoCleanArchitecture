using CleanArchitecture.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using PublicCommon;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CleanArchitecture.WebApi.Infrastructure.Services;


public class AuthenticatedUserService(IHttpContextAccessor httpContextAccessor) : IAuthenticatedUserService
    {
    //public HttpContext HttpContext => httpContextAccessor.HttpContext;
    public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true ?
        httpContextAccessor.HttpContext.User : null;

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    public string UserId { get; } = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    public Guid UserGuId { get; } = Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId) ? userId : Guid.Empty;
    public string UserName { get; } = httpContextAccessor.HttpContext?.User.Identity?.Name;
    public string Name { get; } = httpContextAccessor.HttpContext?.User.FindFirstValue("name");
    public string Email { get; } = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public List<string> Roles => httpContextAccessor.HttpContext?.User?.Roles();

    public bool IsInRole(string role) => httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;

    public bool IsInAnyOfRoles(string[] roles) => roles?.Length > 0 ? httpContextAccessor.HttpContext?.User?.IsInAnyOfRoles(roles) ?? false : false;

    public bool IsInAnyOfRoles(List<string> roles) => roles?.Count > 0 ? IsInAnyOfRoles(roles.ToArray()) : false;

    public bool IsAdminWriters() => httpContextAccessor.HttpContext?.User?.IsInAnyOfRoles(CONSTANTS.ROLES.AdminWriters) ?? false;

    public bool IsTownAdminWriters(int townId = 0) =>
        httpContextAccessor.HttpContext?.User?.IsInAnyOfRoles(CONSTANTS.ROLES.TownAdminWriters(townId)) ?? false;
    }

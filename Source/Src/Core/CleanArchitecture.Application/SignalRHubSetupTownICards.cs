using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MyTown.Application;

public partial class SignalRHubTownICards(IHttpContextAccessor httpContextAccessor,
ITownCardController cardController, ILogger<SignalRHubTownICards> logger)
    //IMediator mediator, IIdentityRepository identityRepository,IAuthenticatedUserService authenticatedUserService,
    {
    //private readonly IMediator _mediator;
    //private readonly IIdentityRepository _identityRepository;
    //private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly ILogger<SignalRHubTownICards> _logger = logger;

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private ITownCardController _cardController = cardController;

    // Property to get the current HttpContext
    private HttpContext? HttpContext => _httpContextAccessor?.HttpContext;

    // Property to get the current user's authentication status
    public bool IsAuthenticated => HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    // Property to get the current user's ID
    public string? UserId => IsAuthenticated ? HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) : default;

    // Property to get the current user's GUID
    public Guid? UserGuId => IsAuthenticated ? Guid.TryParse(HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId) ? userId : Guid.Empty : default;

    // Property to get the current user's name
    public string? UserName => IsAuthenticated ? HttpContext?.User?.Identity?.Name : default;
    }

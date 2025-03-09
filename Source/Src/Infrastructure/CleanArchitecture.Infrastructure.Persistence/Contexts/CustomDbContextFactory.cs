using CleanArchitecture.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Persistence.Contexts;

/// <summary>
/// CustomDbCOntextFactory is only for inside IDbContextFactory
/// ALl place use DbContextProvider Only instead of ApplciaitonDbContext
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="_options"></param>
/// <param name="_authenticatedUserService"></param>
/// <param name="_logger"></param>
public class CustomDbContextFactory<TContext>(DbContextOptions<TContext> _options, IAuthenticatedUserService _authenticatedUserService, ILogger<CustomDbContextFactory<TContext>> _logger)
    : IDbContextFactory<TContext> where TContext : DbContext
    {
    public TContext CreateDbContext()
        {
        //_logger.LogWarning("Creating a new DbContext instance at {Time}", DateTime.UtcNow);
        try
            {
            return (TContext)Activator.CreateInstance(typeof(TContext), _options, _authenticatedUserService);
            }
        catch (Exception e)
            {
            _logger.LogError(e.ToString());
            throw;
            }
        }
    }

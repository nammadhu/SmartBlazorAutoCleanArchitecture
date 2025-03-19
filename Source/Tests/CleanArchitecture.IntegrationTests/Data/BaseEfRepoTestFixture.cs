using BASE.Common;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CleanArchitecture.IntegrationTests.Data;
public abstract class BaseEfRepoTestFixture
{
    protected DbContextProvider dbContextProvider;
    ApplicationDbContext dbContext;
    ILogger<UnitOfWork> logger;

    protected BaseEfRepoTestFixture(DbContextProvider _dbContextProvider, ILogger<UnitOfWork> _logger)
        {
        var options = CreateNewContextOptions();
        IAuthenticatedUserService authenticatedUserService = new AuthenticatedUserService(Guid.NewGuid().ToString(),"UserName");

        dbContext = new ApplicationDbContext(options, authenticatedUserService);
        dbContextProvider = _dbContextProvider;
        logger = _logger;
        }

    protected static DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        // Create a fresh service provider, and therefore a fresh
        // InMemory database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        // Create a new options instance telling the context to use an
        // InMemory database and the new service provider.
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseInMemoryDatabase(nameof(ApplicationDbContext))
               .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }

    protected GenericRepository<T> GetRepository<T>() where T : class,IAuditableBaseEntity
    {
        return new GenericRepository<T>(dbContextProvider);
    }

    protected IUnitOfWork GetUnitOfWork()
    {
        return new UnitOfWork(dbContextProvider, logger);
    }
}

internal record AuthenticatedUserService(string UserId, string UserName) : IAuthenticatedUserService
    {
    public ClaimsPrincipal User => throw new NotImplementedException();
    public List<string> Roles => throw new NotImplementedException();
    public bool IsAuthenticated => throw new NotImplementedException();

    public Guid UserGuId => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public string Email => throw new NotImplementedException();

    public bool IsAdminWriters()
        {
        throw new NotImplementedException();
        }

    public bool IsInAnyOfRoles(string[] roles)
        {
        throw new NotImplementedException();
        }

    public bool IsInAnyOfRoles(List<string> roles)
        {
        throw new NotImplementedException();
        }

    public bool IsInRole(string role)
        {
        throw new NotImplementedException();
        }

    public bool IsTownAdminWriters(int townId = 0)
        {
        throw new NotImplementedException();
        }
    }


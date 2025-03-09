using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PublicCommon.Common;

namespace CleanArchitecture.IntegrationTests.Data;
public abstract class BaseEfRepoTestFixture
{
    protected DbContextProvider dbContextProvider;
    ApplicationDbContext dbContext;
    ILogger<UnitOfWork> logger;

    protected BaseEfRepoTestFixture(DbContextProvider _dbContextProvider, ILogger<UnitOfWork> _logger)
        {
        var options = CreateNewContextOptions();
        IAuthenticatedUserService authenticatedUserService = new AuthenticatedUserService(Guid.NewGuid().ToString(), "UserName");

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
internal record AuthenticatedUserService(string UserId, string UserName) : IAuthenticatedUserService;

namespace CleanArchitecture.Infrastructure.Persistence.Contexts;

/// CustomDbCOntextFactory is only for inside IDbContextFactory
/// ALl place use DbContextProvider Only instead of ApplciaitonDbContext
public class DbContextProvider(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IDisposable
    {
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory;
    private ApplicationDbContext _dbContext;

    public ApplicationDbContext DbContext => _dbContext ??= _dbContextFactory.CreateDbContext();

    public void Dispose()
        {
        _dbContext?.Dispose();
        _dbContext = null;
        }
    }

/*
public class DbContextProvider
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private ApplicationDbContext _dbContext;

    public DbContextProvider(DbContextOptions<ApplicationDbContext> options, IAuthenticatedUserService authenticatedUserService)
    {
        _options = options;
        _authenticatedUserService = authenticatedUserService;
    }

    public ApplicationDbContext DbContext => _dbContext ??= new ApplicationDbContext(_options, _authenticatedUserService);

    public void Dispose()
    {
        _dbContext?.Dispose();
        _dbContext = null;
    }
}*/

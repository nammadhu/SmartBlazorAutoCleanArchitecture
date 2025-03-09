using CleanArchitecture.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Persistence.Contexts;

public class UnitOfWork(DbContextProvider dbContextProvider, ILogger<UnitOfWork> logger) : IUnitOfWork
    {
    private readonly DbContextProvider _dbContextProvider = dbContextProvider;
    private readonly ILogger<UnitOfWork> _logger = logger;

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
        try
            {
            var result = await _dbContextProvider.DbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
            }
        catch (DbUpdateException dbEx)
            {
            _logger.LogError(dbEx, "Database UpdateCard Exception: {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
            throw;
            }
        catch (Exception e)
            {
            _logger.LogError(e, "General Exception: {Message}", e.Message);
            throw;
            }
        finally
            {
            _dbContextProvider.Dispose();
            }
        }

    //private bool SaveChanges()//dont use this
    //    {
    //        try
    //        {
    //            return dbContext.SaveChanges() > 0;
    //        }
    //        catch (Exception e)
    //        {
    //            Console.Write(e.ToString());
    //            throw;
    //        }

    //    }
    }

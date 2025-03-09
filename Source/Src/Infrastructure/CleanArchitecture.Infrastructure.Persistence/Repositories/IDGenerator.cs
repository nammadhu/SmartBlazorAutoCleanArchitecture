using CleanArchitecture.Application.Interfaces;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
    {
    public class IDGenerator<TEntity>(DbContextProvider dbContextProvider, string idColumnName) : IIDGenerator<TEntity> where TEntity : class
        {
        public int GetNextID()
            {
            // Fetch the current maximum ID from the database for the specific table
            var maxId = dbContextProvider.DbContext.Set<TEntity>().Max(entity => EF.Property<int>(entity, idColumnName));
            return maxId + 1; // Ensure returned ID is greater than both internal state and DB max
            }
        }
    }

using System.Linq.Expressions;

namespace CleanArchitecture.Infrastructure.Persistence.Contexts;

public static class DbSetExtensions
    {
    public static async Task AddOrUpdateAsync<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, object>> idSelector,
        CancellationToken cancellationToken) where T : class
        {
        var id = idSelector.Compile()(entity);
        var existingEntity = dbSet.Find(id);

        if (existingEntity == null)
            {
            await dbSet.AddAsync(entity, cancellationToken);
            }
        else
            {
            dbSet.Attach(entity);
            dbSet.Entry(entity).State = EntityState.Modified;
            }
        }

    //keep either of the method as perfectly working the best
    public static async Task<int> AddOrUpdateAndSaveChangesAsync<T>(this DbContext context, T entity, Func<T, object> idSelector, CancellationToken cancellationToken, bool isSaveChanges = true)
           where T : class
        {
        var entry = context.Entry(entity);
        var primaryKey = idSelector(entity);
        if (entry.State == EntityState.Detached || entry.State == EntityState.Added)
            {
            // Check if an entity with the same ID exists
            var existingEntity = context.Set<T>().Local.FirstOrDefault(e => idSelector(e).Equals(primaryKey));
            if (existingEntity != null)
                {
                // UpdateCard the existing entity
                context.Entry(existingEntity).CurrentValues.SetValues(entity);
                }
            else
                {
                // Add the new entity
                await context.Set<T>().AddAsync(entity, cancellationToken);
                }
            }
        else if (entry.State == EntityState.Modified)
            {
            // Entity is already being tracked, no need to check existence
            context.Set<T>().Update(entity);
            }
        // Handle other states (Unchanged, Deleted) if needed
        if (isSaveChanges)
            return await context.SaveChangesAsync(cancellationToken);
        else return 0;
        }
    }

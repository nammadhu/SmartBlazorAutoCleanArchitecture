using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedResponse.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class GenericRepository<T>(DbContextProvider dbContextProvider) : IGenericRepository<T> where T : class, IAuditableBaseEntity
    {
    private readonly DbContextProvider _dbContextProvider = dbContextProvider;

    protected ApplicationDbContext DbContext => _dbContextProvider.DbContext;

    public virtual async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken)
        {
        var result = await DbContext.Set<T>().FindAsync(id, cancellationToken);
        if (result != null)
            DbContext.Entry(result).State = EntityState.Detached; // to avoid error of already tracking object
        return result;
        }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken)
        {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);
        return entity;
        }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
        await DbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entities;
        }

    public void Update(T entity)
        {
        DbContext.Entry(entity).State = EntityState.Modified;
        }

    public void Delete(T entity)
        {
        DbContext.Set<T>().Remove(entity);
        }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken)
        {
        return await DbContext
             .Set<T>()
             .AsNoTracking()
             .ToListAsync(cancellationToken);
        }

    // New method to get records with created or modified later than the given DateTime
    public async Task<IReadOnlyList<T>> GetLatestAllAsync(DateTime dateTimeStamp, CancellationToken cancellationToken = default)
        {
        return await DbContext
            .Set<T>()
            .AsNoTracking()
            .Where(x => x.Created > dateTimeStamp || (x.LastModified.HasValue && x.LastModified.Value > dateTimeStamp))
            .ToListAsync(cancellationToken);
        }

    protected async Task<PaginationResponseDto<TEntity>> Paged<TEntity>(IQueryable<TEntity> query, int pageNumber, int pageSize, bool getTotalCount, CancellationToken cancellationToken = default) where TEntity : class
        {

        var pagedResult = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new(pagedResult, getTotalCount ? await query.CountAsync() : pagedResult.Count, pageNumber, pageSize);
        }
    }

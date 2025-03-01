using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class GenericRepository<T>(DbContext dbContext) : IGenericRepository<T> where T : class, IAuditableBaseEntity
{
    public virtual async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().FindAsync(id, cancellationToken);
    }

    public async Task<T> AddAsync(T entity,CancellationToken cancellationToken = default)
    {
        await dbContext.Set<T>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public void Update(T entity)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync( CancellationToken cancellationToken = default)
    {
        return await dbContext
             .Set<T>()
             .AsNoTracking()
             .ToListAsync(cancellationToken);
    }

    // New method to get records with created or modified later than the given DateTime
    public async Task<IReadOnlyList<T>> GetLatestAllAsync(DateTime dateTimeStamp, CancellationToken cancellationToken = default)
    {
        return await dbContext
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

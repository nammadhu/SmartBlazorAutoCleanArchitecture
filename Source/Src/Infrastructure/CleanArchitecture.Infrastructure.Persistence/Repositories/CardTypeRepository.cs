using AutoMapper;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using MyTown.Application.Interfaces.Repositories;
using MyTown.Domain;
using MyTown.SharedModels.DTOs;
using SharedResponse.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
{
    public class CardTypeRepository(DbContextProvider dbContextProvider, IMapper mapper) : GenericRepository<CardType>(dbContextProvider), ICardTypeRepository
    {
        private const int ResultLimit = 20;
        private readonly DbSet<CardType> db = dbContextProvider.DbContext.Set<CardType>();

        public async Task<PagedResponse<CardTypeDto>> GetPagedListAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken)
        {
            var query = db.OrderBy(p => p.Created).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            return await Paged(
                query.Select(p => mapper.Map<CardTypeDto>(p)),
                pageNumber,
                pageSize, cancellationToken);
        }

        public async Task<IList<CardTypeDto>> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            var query = db.OrderBy(p => p.Created).AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            return await query
                 .OrderByDescending(x => x.Id)
                .Take(ResultLimit).Select(p => mapper.Map<CardTypeDto>(p)).ToListAsync(cancellationToken);
        }

        public async Task<bool> IsNameExistsAsync(string name, CancellationToken cancellationToken)
        {
            return await db.AnyAsync(x => x.Name == name, cancellationToken);
        }
    }
}
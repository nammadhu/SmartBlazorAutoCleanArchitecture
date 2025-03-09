using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using CleanArchitecture.Domain;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class CardDataRepository(DbContextProvider dbContextProvider)
: GenericRepository<CardData>(dbContextProvider: dbContextProvider), ICardDataRepository
{
}
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using MyTown.Application.Interfaces.Repositories;
using MyTown.Domain;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class CardDataRepository(DbContextProvider dbContextProvider)
: GenericRepository<CardData>(dbContextProvider: dbContextProvider), ICardDataRepository
{
}
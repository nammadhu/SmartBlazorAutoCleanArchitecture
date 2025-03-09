namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class CardDataRepository(DbContextProvider dbContextProvider)
: GenericRepository<CardData>(dbContextProvider: dbContextProvider), ICardDataRepository
    {
    }

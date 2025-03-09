namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class ProductRepository(DbContextProvider dbContextProvider) : GenericRepository<Product>(dbContextProvider: dbContextProvider), IProductRepository
    {


    }

using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain;
using CleanArchitecture.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Wrappers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class ProductRepository(DbContextProvider dbContextProvider) : GenericRepository<Product>(dbContextProvider: dbContextProvider), IProductRepository
{
  

    }

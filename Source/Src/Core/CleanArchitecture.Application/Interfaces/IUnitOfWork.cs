namespace CleanArchitecture.Application.Interfaces;

public interface IUnitOfWork
    {
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

namespace CleanArchitecture.Application.Interfaces;

public interface IIDGenerator<TEntity> where TEntity : class
    {
    int GetNextID();
    }

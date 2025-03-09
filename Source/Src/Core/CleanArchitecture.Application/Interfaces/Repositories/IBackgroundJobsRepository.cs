namespace CleanArchitecture.Application.Interfaces.Repositories;

public interface IBackgroundJobsRepository
    {
    Task<int> MarkTownAsCardsUpdated(int townId, CancellationToken cancellationToken);
    }

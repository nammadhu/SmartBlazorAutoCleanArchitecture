using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyTown.SharedModels.Interfaces;

namespace CleanArchitecture.Infrastructure.Persistence;

public class TownHealthCheck : IHealthCheck
{
    private readonly ITownController _townRepository;

    public TownHealthCheck(ITownController townRepository)
    {
        _townRepository = townRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var towns = await _townRepository.GetAll(cancellationToken);
        if (towns?.Success == true && towns.Data != null && towns.Data.Any())
        {
            return HealthCheckResult.Healthy("Town data is loaded correctly." + towns.Data.Count());
        }

        return HealthCheckResult.Unhealthy("Town data is not loaded.");
    }
}
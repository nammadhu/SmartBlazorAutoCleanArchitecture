using CleanArchitecture.Infrastructure.Identity_Azure_AD_B2C;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArchitecture.Infrastructure.Persistence;

public class Azure_AD_B2CHealthCheck : IHealthCheck
{
    private readonly GraphService _graphService;

    public Azure_AD_B2CHealthCheck(GraphService graphService)
    {
        _graphService = graphService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var results = await _graphService.SearchUsersByNameOrEmail("madhu", null, cancellationToken: cancellationToken);
        if (results != null && results.Any())
        {
            return HealthCheckResult.Healthy("Town data is loaded correctly." + results.Count());
        }

        return HealthCheckResult.Unhealthy("Town data is not loaded.");
    }
}
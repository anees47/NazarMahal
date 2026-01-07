using Microsoft.Extensions.Diagnostics.HealthChecks;
using NazarMahal.Infrastructure.Data;

namespace NazarMahal.API.HealthChecks;

public class DatabaseHealthCheck(ApplicationDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            if (canConnect)
            {
                return HealthCheckResult.Healthy("Database is available");
            }
            return HealthCheckResult.Unhealthy("Database is not available");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Database check failed: {ex.Message}", ex);
        }
    }
}

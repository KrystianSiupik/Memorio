using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Limits.Infrastructure.Persistence;

/// <summary>Ready only while the limits database answers: without it no space can be reserved.</summary>
internal sealed class LimitsDatabaseHealthCheck(LimitsDbContext database) : IHealthCheck
{
    public const string Name = "limits-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Limits database unavailable.");
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Succession.Infrastructure.Persistence;

/// <summary>Ready only while the succession database answers.</summary>
internal sealed class SuccessionDatabaseHealthCheck(SuccessionDbContext database) : IHealthCheck
{
    public const string Name = "succession-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Succession database unavailable.");
}

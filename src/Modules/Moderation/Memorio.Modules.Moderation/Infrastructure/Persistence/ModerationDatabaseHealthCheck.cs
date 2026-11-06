using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Moderation.Infrastructure.Persistence;

/// <summary>Ready only while the moderation database answers.</summary>
internal sealed class ModerationDatabaseHealthCheck(ModerationDbContext database) : IHealthCheck
{
    public const string Name = "moderation-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Moderation database unavailable.");
}

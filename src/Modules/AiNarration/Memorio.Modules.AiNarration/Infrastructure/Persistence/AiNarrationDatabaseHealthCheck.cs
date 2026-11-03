using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence;

/// <summary>Ready only while the ainarration database answers: without it no draft can be kept.</summary>
internal sealed class AiNarrationDatabaseHealthCheck(AiNarrationDbContext database) : IHealthCheck
{
    public const string Name = "ai-narration-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("AiNarration database unavailable.");
}

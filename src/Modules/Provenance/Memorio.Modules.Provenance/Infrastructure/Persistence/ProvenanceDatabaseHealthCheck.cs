using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Provenance.Infrastructure.Persistence;

/// <summary>Ready only while the provenance database answers.</summary>
internal sealed class ProvenanceDatabaseHealthCheck(ProvenanceDbContext database) : IHealthCheck
{
    public const string Name = "provenance-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Provenance database unavailable.");
}

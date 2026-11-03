using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Archive.Infrastructure.Persistence;

/// <summary>Ready only while the archive database answers: without it no memory can be read or deleted.</summary>
internal sealed class ArchiveDatabaseHealthCheck(ArchiveDbContext database) : IHealthCheck
{
    public const string Name = "archive-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Archive database unavailable.");
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Album.Infrastructure.Persistence;

/// <summary>Ready only while the album database answers: without it no album can be read.</summary>
internal sealed class AlbumDatabaseHealthCheck(AlbumDbContext database) : IHealthCheck
{
    public const string Name = "album-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Album database unavailable.");
}

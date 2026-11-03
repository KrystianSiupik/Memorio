using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Places.Infrastructure.Persistence;

/// <summary>Ready only while the places database answers: without it no place can be suggested.</summary>
internal sealed class PlacesDatabaseHealthCheck(PlacesDbContext database) : IHealthCheck
{
    public const string Name = "places-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Places database unavailable.");
}

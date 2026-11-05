using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Access.Infrastructure.Persistence;

/// <summary>Ready only while the access database answers: without it no space can be reserved.</summary>
internal sealed class AccessDatabaseHealthCheck(AccessDbContext database) : IHealthCheck
{
    public const string Name = "access-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Access database unavailable.");
}

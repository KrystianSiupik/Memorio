using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Plans.Infrastructure.Persistence;

/// <summary>Ready only while the plans database answers: without the ledger no plan can be read.</summary>
internal sealed class PlansDatabaseHealthCheck(PlansDbContext database) : IHealthCheck
{
    public const string Name = "plans-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Plans database unavailable.");
}

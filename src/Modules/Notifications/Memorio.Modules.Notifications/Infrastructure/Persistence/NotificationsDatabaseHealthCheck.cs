using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Notifications.Infrastructure.Persistence;

/// <summary>Ready only while the notifications database answers: without it no space can be reserved.</summary>
internal sealed class NotificationsDatabaseHealthCheck(NotificationsDbContext database) : IHealthCheck
{
    public const string Name = "notifications-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Notifications database unavailable.");
}

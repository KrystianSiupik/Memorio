using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Memorio.Hosting.Messaging;

/// <summary>Ready only while the messaging database answers (RD4): without it no event can be stored or delivered.</summary>
internal sealed class MessagingDatabaseHealthCheck(MessagingDatabase database) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var command = database.DataSource.CreateCommand("SELECT 1");
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (NpgsqlException exception)
        {
            return HealthCheckResult.Unhealthy("Messaging database unavailable.", exception);
        }
    }
}

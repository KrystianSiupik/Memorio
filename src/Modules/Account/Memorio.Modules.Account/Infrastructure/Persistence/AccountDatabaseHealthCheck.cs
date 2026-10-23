using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Modules.Account.Infrastructure.Persistence;

/// <summary>Ready only while the account database answers: nobody can sign in without it.</summary>
internal sealed class AccountDatabaseHealthCheck(AccountDbContext database) : IHealthCheck
{
    public const string Name = "account-database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Account database unavailable.");
}

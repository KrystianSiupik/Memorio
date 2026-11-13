using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Memorio.Modularity;

/// <summary>
/// Migrations of every module, in every host, one at a time (D-373). The API and the worker start together
/// and both apply the migrations; EF Core does not keep two processes from applying the same migration on
/// PostgreSQL, so the slower one failed with "already exists" and stopped. A session advisory lock on one
/// key for the whole database makes the second process wait and then find nothing left to apply. The lock
/// goes with the session, so a host that dies while migrating does not keep it.
/// </summary>
public static class OneMigrationAtATime
{
    /// <summary>The same key in every module and host: "MEMORIO" as a number, only for these migrations.</summary>
    public const long LockKey = 0x4D454D4F52494F;

    public static async Task MigrateOneAtATimeAsync(this DatabaseFacade database, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(database);

        await database.OpenConnectionAsync(cancellationToken);
        try
        {
            await database.ExecuteSqlAsync($"SELECT pg_advisory_lock({LockKey})", cancellationToken);
            try
            {
                await database.MigrateAsync(cancellationToken);
            }
            finally
            {
                await database.ExecuteSqlAsync($"SELECT pg_advisory_unlock({LockKey})", CancellationToken.None);
            }
        }
        finally
        {
            await database.CloseConnectionAsync();
        }
    }
}

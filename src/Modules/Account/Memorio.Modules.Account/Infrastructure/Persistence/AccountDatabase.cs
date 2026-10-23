using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Memorio.Modules.Account.Infrastructure.Persistence;

internal static class AccountDatabase
{
    /// <summary>Npgsql on the module's schema, migrations history included; no Kerberos (D-141).</summary>
    public static DbContextOptionsBuilder UseAccountDatabase(this DbContextOptionsBuilder options, string connectionString) =>
        options.UseNpgsql(
            new NpgsqlConnectionStringBuilder(connectionString) { GssEncryptionMode = GssEncryptionMode.Disable }.ConnectionString,
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", AccountDbContext.Schema));
}

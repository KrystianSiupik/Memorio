using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Memorio.Modules.Account.Infrastructure.Persistence;

internal static class AccountDatabase
{
    /// <summary>
    /// Npgsql on the module's schema, migrations history included; no Kerberos (D-141). One pool per process
    /// (EF Core shares the data source of a connection string), named after the schema in the pool metrics (D-181).
    /// </summary>
    public static DbContextOptionsBuilder UseAccountDatabase(this DbContextOptionsBuilder options, string connectionString) =>
        options.UseNpgsql(
            new NpgsqlConnectionStringBuilder(connectionString) { GssEncryptionMode = GssEncryptionMode.Disable }.ConnectionString,
            npgsql => npgsql
                .MigrationsHistoryTable("__EFMigrationsHistory", AccountDbContext.Schema)
                .ConfigureDataSource(dataSource => dataSource.Name = AccountDbContext.Schema));
}

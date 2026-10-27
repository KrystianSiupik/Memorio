using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Memorio.Modules.Plans.Infrastructure.Persistence;

internal static class PlansDatabase
{
    /// <summary>
    /// Npgsql on the module's schema, migrations history included; no Kerberos (D-141). One pool per process
    /// (EF Core shares the data source of a connection string), named after the schema in the pool metrics (D-181).
    /// </summary>
    public static DbContextOptionsBuilder UsePlansDatabase(this DbContextOptionsBuilder options, string connectionString) =>
        options.UseNpgsql(
            new NpgsqlConnectionStringBuilder(connectionString) { GssEncryptionMode = GssEncryptionMode.Disable }.ConnectionString,
            npgsql => npgsql
                .MigrationsHistoryTable("__EFMigrationsHistory", PlansDbContext.Schema)
                .ConfigureDataSource(dataSource => dataSource.Name = PlansDbContext.Schema));
}

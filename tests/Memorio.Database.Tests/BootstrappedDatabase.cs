using Memorio.Testing;
using Npgsql;

[assembly: AssemblyFixture(typeof(Memorio.Database.Tests.BootstrappedDatabase))]

namespace Memorio.Database.Tests;

/// <summary>
/// The bootstrapped database with one probe table in every module schema, created by the module's own
/// role, and one in each messaging schema, created by memorio_messaging (as Wolverine creates its tables).
/// </summary>
public sealed class BootstrappedDatabase : PostgresDatabase
{
    public const string ProbeTable = "fitness_probe";

    public static readonly string[] MessagingSchemas = ["wolverine", "wolverine_queues"];

    protected override async Task OnBootstrappedAsync()
    {
        foreach (var schema in ModuleSchemas.All)
        {
            await CreateProbeAsync(ModuleSchemas.RoleOf(schema), ProbeTable);
        }

        foreach (var schema in MessagingSchemas)
        {
            await CreateProbeAsync(MessagingRole, $"{schema}.{ProbeTable}");
        }
    }

    public Task<NpgsqlConnection> OpenAsModuleAsync(string schema) => OpenAsAsync(ModuleSchemas.RoleOf(schema));

    private async Task CreateProbeAsync(string role, string table)
    {
        await using var connection = await OpenAsAsync(role);
        await using var command = new NpgsqlCommand($"CREATE TABLE {table} (id int); INSERT INTO {table} VALUES (1)", connection);
        await command.ExecuteNonQueryAsync();
    }
}

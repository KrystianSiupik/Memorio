using Memorio.Testing;
using Npgsql;
using static Memorio.Database.Tests.BootstrappedDatabase;

namespace Memorio.Database.Tests;

/// <summary>
/// Fitness function SD4 (ADR 0004): a module reads and writes only its own schema. Checked with the
/// real role rights after db/bootstrap.sql, so a grant added by mistake fails CI.
/// </summary>
public sealed class ModuleSchemaIsolationTests(BootstrappedDatabase database)
{
    private const string InsufficientPrivilege = "42501";

    public static TheoryData<string> Schemas => [.. ModuleSchemas.All];

    [Fact]
    public async Task Bootstrap_creates_one_schema_per_module_and_the_messaging_schemas_each_owned_by_its_role()
    {
        await using var connection = await database.OpenAsOwnerAsync();
        await using var command = new NpgsqlCommand(
            """
            SELECT n.nspname, pg_get_userbyid(n.nspowner)
            FROM pg_namespace n
            WHERE pg_get_userbyid(n.nspowner) LIKE 'memorio\_%' AND pg_get_userbyid(n.nspowner) <> current_user
            ORDER BY n.nspname COLLATE "C"
            """,
            connection);
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);
        var schemas = new List<(string Schema, string Owner)>();
        while (await reader.ReadAsync(TestContext.Current.CancellationToken))
        {
            schemas.Add((reader.GetString(0), reader.GetString(1)));
        }

        var expected = ModuleSchemas.All.Select(schema => (schema, ModuleSchemas.RoleOf(schema)))
            .Concat(MessagingSchemas.Select(schema => (schema, PostgresDatabase.MessagingRole)))
            .OrderBy(owned => owned.Item1, StringComparer.Ordinal);
        Assert.Equal(expected, schemas);
    }

    [Theory]
    [MemberData(nameof(Schemas))]
    public async Task A_module_reads_and_writes_its_own_schema(string schema)
    {
        await using var connection = await database.OpenAsModuleAsync(schema);

        // Unqualified names resolve to the module schema (search_path set by the bootstrap).
        await ExecuteAsync(connection, $"INSERT INTO {ProbeTable} VALUES (2)");
        await ExecuteAsync(connection, $"CREATE TABLE IF NOT EXISTS {schema}.own_table (id int)");

        Assert.True(await ScalarAsync<long>(connection, $"SELECT count(*) FROM {schema}.{ProbeTable}") >= 1);
    }

    [Theory]
    [MemberData(nameof(Schemas))]
    public async Task A_module_cannot_read_write_or_create_in_another_module_schema(string schema)
    {
        await using var connection = await database.OpenAsModuleAsync(schema);

        foreach (var other in ModuleSchemas.All.Where(other => other != schema))
        {
            await AssertDeniedAsync(connection, $"SELECT * FROM {other}.{ProbeTable}");
            await AssertDeniedAsync(connection, $"INSERT INTO {other}.{ProbeTable} VALUES (3)");
            await AssertDeniedAsync(connection, $"CREATE TABLE {other}.intruder (id int)");
        }
    }

    [Theory]
    [MemberData(nameof(Schemas))]
    public async Task A_module_cannot_create_tables_in_public(string schema)
    {
        await using var connection = await database.OpenAsModuleAsync(schema);

        await AssertDeniedAsync(connection, "CREATE TABLE public.intruder (id int)");
    }

    [Fact]
    public async Task A_module_uses_PostGIS_from_public()
    {
        await using var connection = await database.OpenAsModuleAsync("places");

        Assert.Equal("POINT(20 49)", await ScalarAsync<string>(connection, "SELECT ST_AsText(ST_MakePoint(20, 49))"));
    }

    [Fact]
    public async Task The_bootstrap_runs_again_without_losing_data()
    {
        await database.ApplyBootstrapAsync();

        await using var connection = await database.OpenAsModuleAsync("album");
        Assert.True(await ScalarAsync<long>(connection, $"SELECT count(*) FROM {ProbeTable}") >= 1);
    }

    [Theory]
    [MemberData(nameof(Schemas))]
    public async Task A_module_only_adds_rows_to_the_outbox_tables_and_never_touches_the_queues(string schema)
    {
        await using var connection = await database.OpenAsModuleAsync(schema);

        // Outbox in the module's transaction (ADR 0006): adding an envelope is the only right.
        await ExecuteAsync(connection, $"INSERT INTO wolverine.{ProbeTable} VALUES (2)");
        await AssertDeniedAsync(connection, $"SELECT * FROM wolverine.{ProbeTable}");
        await AssertDeniedAsync(connection, $"UPDATE wolverine.{ProbeTable} SET id = 3");
        await AssertDeniedAsync(connection, $"DELETE FROM wolverine.{ProbeTable}");
        await AssertDeniedAsync(connection, "CREATE TABLE wolverine.intruder (id int)");
        await AssertDeniedAsync(connection, $"SELECT * FROM wolverine_queues.{ProbeTable}");
        await AssertDeniedAsync(connection, $"INSERT INTO wolverine_queues.{ProbeTable} VALUES (2)");
    }

    private static async Task AssertDeniedAsync(NpgsqlConnection connection, string sql)
    {
        var error = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(connection, sql));
        Assert.True(error.SqlState == InsufficientPrivilege, $"{sql}: expected permission denied, got {error.SqlState} {error.MessageText}");
    }

    private static async Task ExecuteAsync(NpgsqlConnection connection, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
    }

    private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken))!;
    }
}

using Npgsql;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(Memorio.Database.Tests.BootstrappedDatabase))]

namespace Memorio.Database.Tests;

/// <summary>
/// A PostgreSQL + PostGIS container (the image of compose.yaml) with db/bootstrap.sql applied the way
/// every environment applies it (psql as the database owner), and one probe table in every module
/// schema, created by the module's own role. Needs Docker.
/// </summary>
public sealed class BootstrappedDatabase : IAsyncLifetime
{
    public const string ProbeTable = "fitness_probe";

    private const string ModuleRolePassword = "module-role-test";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgis/postgis:18-3.6")
        .WithDatabase("memorio")
        .WithUsername("memorio_admin")
        .WithPassword("memorio_test")
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        await ApplyBootstrapAsync();

        foreach (var schema in ModuleSchemas.All)
        {
            await ExecuteAsOwnerAsync($"ALTER ROLE {ModuleSchemas.RoleOf(schema)} PASSWORD '{ModuleRolePassword}'");

            await using var connection = await OpenAsModuleAsync(schema);
            await using var command = new NpgsqlCommand(
                $"CREATE TABLE {ProbeTable} (id int); INSERT INTO {ProbeTable} VALUES (1)", connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async ValueTask DisposeAsync() => await _container.DisposeAsync();

    /// <summary>Runs db/bootstrap.sql through psql, stopping at the first error.</summary>
    public async Task ApplyBootstrapAsync()
    {
        var script = await File.ReadAllTextAsync(Path.Combine(RepositoryRoot(), "db", "bootstrap.sql"));
        var result = await _container.ExecScriptAsync("\\set ON_ERROR_STOP on\n" + script);

        Assert.True(result.ExitCode == 0, $"db/bootstrap.sql failed: {result.Stderr}");
    }

    public async Task<NpgsqlConnection> OpenAsOwnerAsync()
    {
        var connection = new NpgsqlConnection(_container.GetConnectionString());
        await connection.OpenAsync();
        return connection;
    }

    public async Task<NpgsqlConnection> OpenAsModuleAsync(string schema)
    {
        var connectionString = new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Username = ModuleSchemas.RoleOf(schema),
            Password = ModuleRolePassword,
            Pooling = false,
        };
        var connection = new NpgsqlConnection(connectionString.ConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    private async Task ExecuteAsOwnerAsync(string sql)
    {
        await using var connection = await OpenAsOwnerAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Memorio.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("Memorio.slnx not found above the test output.");
    }
}

using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Memorio.Testing;

/// <summary>
/// A PostgreSQL + PostGIS container (the image of compose.yaml) with db/bootstrap.sql applied the way
/// every environment applies it (psql as the database owner) and a test password for every Memorio
/// role. Needs Docker.
/// </summary>
public class PostgresDatabase : IAsyncLifetime
{
    public const string MessagingRole = "memorio_messaging";
    public const string SchedulerRole = "memorio_scheduler";

    private const string RolePassword = "role-test";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgis/postgis:18-3.6")
        .WithDatabase("memorio")
        .WithUsername("memorio_admin")
        .WithPassword("memorio_test")
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        await ApplyBootstrapAsync();

        foreach (var role in ModuleSchemas.All.Select(ModuleSchemas.RoleOf).Append(MessagingRole).Append(SchedulerRole))
        {
            await using var connection = await OpenAsOwnerAsync();
            await using var command = new NpgsqlCommand($"ALTER ROLE {role} PASSWORD '{RolePassword}'", connection);
            await command.ExecuteNonQueryAsync();
        }

        await OnBootstrappedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await OnDisposingAsync();
        await _container.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    /// <summary>Runs db/bootstrap.sql through psql, stopping at the first error.</summary>
    public async Task ApplyBootstrapAsync()
    {
        var script = await File.ReadAllTextAsync(Path.Combine(RepositoryRoot(), "db", "bootstrap.sql"));
        var result = await _container.ExecScriptAsync("\\set ON_ERROR_STOP on\n" + script);

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException($"db/bootstrap.sql failed: {result.Stderr}");
        }
    }

    /// <summary>Connection string of a Memorio role (a module role, <see cref="MessagingRole"/> or <see cref="SchedulerRole"/>).</summary>
    public string ConnectionStringFor(string role) =>
        new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Username = role,
            Password = RolePassword,
        }.ConnectionString;

    public async Task<NpgsqlConnection> OpenAsOwnerAsync()
    {
        var connection = new NpgsqlConnection(_container.GetConnectionString());
        await connection.OpenAsync();
        return connection;
    }

    public async Task<NpgsqlConnection> OpenAsAsync(string role)
    {
        var connection = new NpgsqlConnection(ConnectionStringFor(role));
        await connection.OpenAsync();
        return connection;
    }

    /// <summary>Extra setup of a test assembly, once the bootstrap ran.</summary>
    protected virtual Task OnBootstrappedAsync() => Task.CompletedTask;

    /// <summary>Stops what <see cref="OnBootstrappedAsync"/> started, before the container goes.</summary>
    protected virtual Task OnDisposingAsync() => Task.CompletedTask;

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

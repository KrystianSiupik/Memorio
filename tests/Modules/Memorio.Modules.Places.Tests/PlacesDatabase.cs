using Memorio.Modules.Places.Infrastructure.Persistence;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Places.Tests.PlacesDatabase))]

namespace Memorio.Modules.Places.Tests;

/// <summary>
/// The bootstrapped database with the module's migrations applied by its own role (as a host applies them),
/// and the module's services on it, registered by <see cref="PlacesModule"/> itself.
/// </summary>
public sealed class PlacesDatabase : PostgresDatabase
{
    public const string Role = "memorio_places";

    private ServiceProvider _services = null!;

    public AsyncServiceScope CreateScope() => _services.CreateAsyncScope();

    /// <summary>A place as the OpenStreetMap import stores it (D-131).</summary>
    public async Task AddOsmPlaceAsync(long osmId, string name, double latitude = 50.06, double longitude = 19.94)
    {
        await using var connection = await OpenAsAsync(Role);
        await using var insert = new Npgsql.NpgsqlCommand(
            """
            INSERT INTO places.osm_places (osm_type, osm_id, name, kind, tags, geom)
            VALUES ('N', @id, @name, 'place=city', '{}', ST_SetSRID(ST_MakePoint(@longitude, @latitude), 4326))
            """,
            connection);
        insert.Parameters.AddWithValue("id", osmId);
        insert.Parameters.AddWithValue("name", name);
        insert.Parameters.AddWithValue("longitude", longitude);
        insert.Parameters.AddWithValue("latitude", latitude);
        await insert.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
    }

    /// <summary>An own place as the module stores it.</summary>
    public async Task AddOwnPlaceAsync(Guid owner, string name)
    {
        await using var connection = await OpenAsAsync(Role);
        await using var insert = new Npgsql.NpgsqlCommand(
            """
            INSERT INTO places.own_places (id, owner_account_id, name, latitude, longitude, created_at)
            VALUES (@id, @owner, @name, 49.3, 19.95, now())
            """,
            connection);
        insert.Parameters.AddWithValue("id", Guid.CreateVersion7());
        insert.Parameters.AddWithValue("owner", owner);
        insert.Parameters.AddWithValue("name", name);
        await insert.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
    }

    protected override async Task OnBootstrappedAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Places:ConnectionString"] = ConnectionStringFor(Role),
                ["Database:MigrateOnStartup"] = "false",
            })
            .Build();
        var services = new ServiceCollection();
        new PlacesModule().AddServices(services, configuration);
        _services = services.BuildServiceProvider();

        await using var scope = CreateScope();
        await scope.ServiceProvider.GetRequiredService<PlacesDbContext>().Database.MigrateAsync();
    }

    protected override async Task OnDisposingAsync() => await _services.DisposeAsync();
}

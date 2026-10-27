using Memorio.Testing;
using Npgsql;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Access.Tests.AccessDatabase))]

namespace Memorio.Modules.Access.Tests;

/// <summary>
/// The bootstrapped database with the probe tables of <see cref="ProbeAlbums"/>: grants in the access
/// schema, created by memorio_access, and albums in the album schema, created by memorio_album. Each role
/// keeps to its own schema (ADR 0004), as the real modules will.
/// </summary>
public sealed class AccessDatabase : PostgresDatabase
{
    public NpgsqlDataSource Access { get; private set; } = null!;

    public NpgsqlDataSource Album { get; private set; } = null!;

    protected override async Task OnBootstrappedAsync()
    {
        Access = NpgsqlDataSource.Create(ConnectionStringFor(ModuleSchemas.RoleOf("access")));
        Album = NpgsqlDataSource.Create(ConnectionStringFor(ModuleSchemas.RoleOf("album")));

        await using (var command = Access.CreateCommand(
            "CREATE TABLE access.probe_grants (person_id uuid NOT NULL, album_id uuid NOT NULL, PRIMARY KEY (person_id, album_id))"))
        {
            await command.ExecuteNonQueryAsync();
        }

        await using (var command = Album.CreateCommand("CREATE TABLE album.probe_albums (id uuid PRIMARY KEY, title text NOT NULL)"))
        {
            await command.ExecuteNonQueryAsync();
        }
    }

    protected override async Task OnDisposingAsync()
    {
        await Access.DisposeAsync();
        await Album.DisposeAsync();
    }
}

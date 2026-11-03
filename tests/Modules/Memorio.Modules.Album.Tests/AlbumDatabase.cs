using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Album.Infrastructure.Persistence;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Album.Tests.AlbumDatabase))]

namespace Memorio.Modules.Album.Tests;

/// <summary>
/// The bootstrapped database with the module's migrations applied by its own role (as a host applies them),
/// and the module's services on it, registered by <see cref="AlbumModule"/> itself.
/// </summary>
public sealed class AlbumDatabase : PostgresDatabase
{
    public const string Role = "memorio_album";

    private ServiceProvider _services = null!;

    public AsyncServiceScope CreateScope() => _services.CreateAsyncScope();

    internal async Task AddAsync(SharedAlbum album)
    {
        await using var scope = CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<AlbumDbContext>();
        database.Albums.Add(album);
        await database.SaveChangesAsync();
    }

    protected override async Task OnBootstrappedAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Album:ConnectionString"] = ConnectionStringFor(Role),
                ["Database:MigrateOnStartup"] = "false",
            })
            .Build();
        var services = new ServiceCollection();
        new AlbumModule().AddServices(services, configuration);
        _services = services.BuildServiceProvider();

        await using var scope = CreateScope();
        await scope.ServiceProvider.GetRequiredService<AlbumDbContext>().Database.MigrateAsync();
    }

    protected override async Task OnDisposingAsync() => await _services.DisposeAsync();
}

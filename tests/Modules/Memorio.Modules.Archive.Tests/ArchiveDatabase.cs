using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.SharedKernel.Storage;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Archive.Tests.ArchiveDatabase))]

namespace Memorio.Modules.Archive.Tests;

/// <summary>
/// The bootstrapped database with the module's migrations applied by its own role (as a host applies them),
/// and the module's services on it, registered by <see cref="ArchiveModule"/> itself.
/// </summary>
public sealed class ArchiveDatabase : PostgresDatabase
{
    public const string Role = "memorio_archive";

    private ServiceProvider _services = null!;

    /// <summary>The object storage of the module, in memory.</summary>
    public FakeObjectStorage Storage { get; } = new();

    public AsyncServiceScope CreateScope() => _services.CreateAsyncScope();

    /// <summary>Stores a memory of the account with the given number of photos.</summary>
    internal async Task<Memory> AddMemoryAsync(Guid accountId, int photos = 0)
    {
        var memory = Memory.Create(accountId, DateTimeOffset.UtcNow);
        for (var photo = 0; photo < photos; photo++)
        {
            memory.AddPhoto($"photos/{Guid.NewGuid():N}/original.jpg");
        }

        await using var scope = CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<ArchiveDbContext>();
        database.Memories.Add(memory);
        await database.SaveChangesAsync();
        return memory;
    }

    protected override async Task OnBootstrappedAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Archive:ConnectionString"] = ConnectionStringFor(Role),
                ["Database:MigrateOnStartup"] = "false",
            })
            .Build();
        var services = new ServiceCollection();
        new ArchiveModule().AddServices(services, configuration);
        services.AddSingleton<IObjectStorage>(Storage);
        _services = services.BuildServiceProvider();

        await using var scope = CreateScope();
        await scope.ServiceProvider.GetRequiredService<ArchiveDbContext>().Database.MigrateAsync();
    }

    protected override async Task OnDisposingAsync() => await _services.DisposeAsync();
}

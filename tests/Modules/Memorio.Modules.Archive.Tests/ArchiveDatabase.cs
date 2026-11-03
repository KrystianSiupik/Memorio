using Memorio.Hosting.Messaging;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Places.Contracts;
using Memorio.SharedKernel.Storage;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Archive.Tests.ArchiveDatabase))]

namespace Memorio.Modules.Archive.Tests;

/// <summary>
/// The bootstrapped database with the module's migrations applied by its own role, and the module's
/// services on it, as the API hosts them with the outbox. The other modules it asks answer from memory.
/// </summary>
public sealed class ArchiveDatabase : PostgresDatabase
{
    public const string Role = "memorio_archive";

    private IHost? _host;

    /// <summary>The object storage of the module, in memory.</summary>
    public FakeObjectStorage Storage { get; } = new();

    public FakeNeighbours Neighbours { get; } = new();

    public AsyncServiceScope CreateScope() => _host!.Services.CreateAsyncScope();

    /// <summary>Stores a memory of the account with the given number of photos.</summary>
    internal async Task<Memory> AddMemoryAsync(Guid accountId, int photos = 0)
    {
        var date = MemoryDate.Create("Day", "2026-07-01", null, new DateOnly(2026, 11, 3)).Value;
        var memory = Memory.Create(
            new MemoryId(Guid.CreateVersion7()), accountId, new MemoryPlace("osm:N:1", "Giewont", 49.25, 19.93), date, DateTimeOffset.UtcNow);
        for (var photo = 0; photo < photos; photo++)
        {
            memory.AddPhoto(Guid.CreateVersion7(), $"photos/{Guid.NewGuid():N}/original.jpg", 100, DateTimeOffset.UtcNow);
        }

        await using var scope = CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<ArchiveDbContext>();
        database.Memories.Add(memory);
        await database.SaveChangesAsync();
        return memory;
    }

    protected override async Task OnBootstrappedAsync()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration["Archive:ConnectionString"] = ConnectionStringFor(Role);
        builder.Configuration["Messaging:ConnectionString"] = ConnectionStringFor(MessagingRole);
        builder.Configuration["Database:MigrateOnStartup"] = "false";
        builder.Services.AddMemorioMessaging(builder.Configuration, MessagingHost.Api, []);
        new ArchiveModule().AddServices(builder.Services, builder.Configuration);
        builder.Services.AddSingleton<IObjectStorage>(Storage);
        builder.Services.AddSingleton<IPlaceReader>(Neighbours);
        builder.Services.AddSingleton<ITermsReader>(Neighbours);
        builder.Services.AddSingleton<ISpacePool>(Neighbours);
        _host = builder.Build();

        await using (var scope = CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<ArchiveDbContext>().Database.MigrateAsync();
        }

        await _host.StartAsync();
    }

    protected override async Task OnDisposingAsync()
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}

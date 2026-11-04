using Memorio.Hosting.Messaging;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Album.Infrastructure.Persistence;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Notifications.Contracts;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Scheduling;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Album.Tests.AlbumDatabase))]

namespace Memorio.Modules.Album.Tests;

/// <summary>
/// The bootstrapped database with the module's migrations applied by its own role, and the module's services
/// on it, as the API hosts them with the outbox. The other modules it asks answer from memory.
/// </summary>
public sealed class AlbumDatabase : PostgresDatabase
{
    public const string Role = "memorio_album";

    private IHost? _host;

    public FakeAlbumNeighbours Neighbours { get; } = new();

    public FakeDeadlines Deadlines { get; } = new();

    public AsyncServiceScope CreateScope() => _host!.Services.CreateAsyncScope();

    internal async Task AddAsync(SharedAlbum album)
    {
        await using var scope = CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<AlbumDbContext>();
        database.Albums.Add(album);
        await database.SaveChangesAsync();
    }

    /// <summary>How many events about this id wait in the worker's queue: nothing consumes it here.</summary>
    public async Task<long> QueuedEventsAsync(string eventName, Guid id)
    {
        await using var messaging = await OpenAsAsync(MessagingRole);
        await using var command = new Npgsql.NpgsqlCommand(
            $"""
            SELECT count(*) FROM {MemorioMessaging.QueueSchema}.wolverine_queue_{MemorioMessaging.Queue}
            WHERE message_type LIKE @type AND position(convert_to(@id, 'UTF8') in body) > 0
            """,
            messaging);
        command.Parameters.AddWithValue("type", "%" + eventName + "%");
        command.Parameters.AddWithValue("id", id.ToString());
        return (long)(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken))!;
    }

    /// <summary>Polls until it holds or a minute (QS7) passes.</summary>
    public static async Task<bool> EventuallyAsync(Func<Task<bool>> condition)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromMinutes(1);
        while (DateTime.UtcNow < deadline)
        {
            if (await condition())
            {
                return true;
            }

            await Task.Delay(200, TestContext.Current.CancellationToken);
        }

        return await condition();
    }

    protected override async Task OnBootstrappedAsync()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration["Album:ConnectionString"] = ConnectionStringFor(Role);
        builder.Configuration["Messaging:ConnectionString"] = ConnectionStringFor(MessagingRole);
        builder.Configuration["Database:MigrateOnStartup"] = "false";
        builder.Services.AddMemorioMessaging(builder.Configuration, MessagingHost.Api, []);
        new AlbumModule().AddServices(builder.Services, builder.Configuration);
        builder.Services.AddSingleton<IAlbumSeatLimit>(Neighbours);
        builder.Services.AddSingleton<IInvitationBlocks>(Neighbours);
        builder.Services.AddSingleton<IAccountDirectory>(Neighbours);
        builder.Services.AddSingleton<IAlbumContent>(Neighbours);
        builder.Services.AddSingleton<IAlbumPool>(Neighbours);
        builder.Services.AddSingleton<IDeadlines>(Deadlines);
        _host = builder.Build();

        await using (var scope = CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AlbumDbContext>().Database.MigrateAsync();
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

using Memorio.Hosting.Messaging;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Moderation.Infrastructure.Persistence;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Moderation.Tests.ModerationTestDatabase))]

namespace Memorio.Modules.Moderation.Tests;

/// <summary>The bootstrapped database with the module's migrations applied by its own role, with the outbox as in the API.</summary>
public sealed class ModerationTestDatabase : PostgresDatabase
{
    public const string Role = "memorio_moderation";

    private IHost? _host;

    public FakeModerationNeighbours Neighbours { get; } = new();

    public AsyncServiceScope CreateScope() => _host!.Services.CreateAsyncScope();

    /// <summary>How many events about this id wait in the worker's queue: nothing consumes it here.</summary>
    public async Task<long> QueuedEventsAsync(string eventName, Guid id)
    {
        await using var messaging = await OpenAsAsync(MessagingRole);
        await using var command = new NpgsqlCommand(
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
        builder.Configuration["Moderation:ConnectionString"] = ConnectionStringFor(Role);
        builder.Configuration["Messaging:ConnectionString"] = ConnectionStringFor(MessagingRole);
        builder.Configuration["Database:MigrateOnStartup"] = "false";
        builder.Services.AddMemorioMessaging(builder.Configuration, MessagingHost.Api, []);
        new ModerationModule().AddServices(builder.Services, builder.Configuration);
        builder.Services.AddSingleton<IAlbumMembership>(Neighbours);
        builder.Services.AddSingleton<IAccessReader>(Neighbours);
        builder.Services.AddSingleton<IReportedMemories>(Neighbours);
        _host = builder.Build();

        await using (var scope = CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<ModerationDbContext>().Database.MigrateAsync();
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

using Memorio.Hosting.Messaging;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Notifications.Infrastructure.Persistence;
using Memorio.Modules.Succession.Contracts;
using Memorio.SharedKernel.Scheduling;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Notifications.Tests.NotificationsDatabase))]

namespace Memorio.Modules.Notifications.Tests;

/// <summary>The bootstrapped database with the module's migrations applied by its own role, with the outbox; messages only recorded.</summary>
public sealed class NotificationsDatabase : PostgresDatabase
{
    public const string Role = "memorio_notifications";

    private IHost? _host;

    public FakeNotificationNeighbours Neighbours { get; } = new();

    public FakeDeadlines Deadlines { get; } = new();

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
        builder.Configuration["Notifications:ConnectionString"] = ConnectionStringFor(Role);
        builder.Configuration["Messaging:ConnectionString"] = ConnectionStringFor(MessagingRole);
        builder.Configuration["Database:MigrateOnStartup"] = "false";
        builder.Services.AddMemorioMessaging(builder.Configuration, MessagingHost.Api, []);
        builder.Services.AddSingleton<INotificationSender>(Neighbours);
        new NotificationsModule().AddServices(builder.Services, builder.Configuration);
        builder.Services.AddSingleton<IAlbumMessaging>(Neighbours);
        builder.Services.AddSingleton<IAccountDirectory>(Neighbours);
        builder.Services.AddSingleton<IDeadlines>(Deadlines);
        builder.Services.AddSingleton<ITakeoverOffers>(Neighbours);
        builder.Services.AddSingleton<ILegacyContacts>(Neighbours);
        _host = builder.Build();

        await using (var scope = CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<NotificationsDbContext>().Database.MigrateAsync();
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

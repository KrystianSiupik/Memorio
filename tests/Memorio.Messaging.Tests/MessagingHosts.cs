using Memorio.Hosting.Messaging;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Wolverine.EntityFrameworkCore;

[assembly: AssemblyFixture(typeof(Memorio.Messaging.Tests.MessagingHosts))]

namespace Memorio.Messaging.Tests;

/// <summary>
/// The bootstrapped database with both sides of ADR 0006 running against it, configured as the real
/// hosts are (<see cref="MemorioMessaging.AddMemorioMessaging"/>): the API side with a module DbContext (album
/// role) and the outbox, and the worker side with this assembly's handlers.
/// </summary>
public sealed class MessagingHosts : PostgresDatabase
{
    /// <summary>Short waits between attempts, so a failing handler reaches the dead letters within a second.</summary>
    public static readonly TimeSpan[] RetryDelays = [TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100)];

    private IHost? _api;
    private IHost? _worker;

    internal HandledEvents Handled { get; } = new();

    internal IServiceProvider Api => _api!.Services;

    protected override async Task OnBootstrappedAsync()
    {
        await using (var album = await OpenAsAsync("memorio_album"))
        await using (var create = new NpgsqlCommand(AlbumProbeContext.CreateTable, album))
        {
            await create.ExecuteNonQueryAsync();
        }

        _worker = Build(MessagingHost.Worker, services => services.AddSingleton(Handled));
        await _worker.StartAsync();

        var albumConnection = ConnectionStringFor("memorio_album");
        _api = Build(MessagingHost.Api, services =>
            services.AddDbContextWithWolverineIntegration<AlbumProbeContext>(options => options.UseNpgsql(albumConnection)));
        await _api.StartAsync();
    }

    protected override async Task OnDisposingAsync()
    {
        foreach (var host in new[] { _api, _worker })
        {
            if (host is not null)
            {
                await host.StopAsync();
                host.Dispose();
            }
        }
    }

    /// <summary>Polls <paramref name="condition"/> until it holds or <paramref name="timeout"/> passes.</summary>
    internal static async Task<bool> EventuallyAsync(Func<Task<bool>> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
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

    private IHost Build(MessagingHost side, Action<IServiceCollection> configure)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration["Messaging:ConnectionString"] = ConnectionStringFor(MessagingRole);
        builder.Services.AddMemorioMessaging(
            builder.Configuration,
            side,
            side == MessagingHost.Worker ? [typeof(ProbeRecordedHandler).Assembly] : [],
            RetryDelays);
        configure(builder.Services);
        return builder.Build();
    }
}

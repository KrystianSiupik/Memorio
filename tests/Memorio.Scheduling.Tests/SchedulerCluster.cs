using Memorio.Hosting.Scheduling;
using Memorio.Modularity;
using Memorio.SharedKernel.Scheduling;
using Memorio.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

[assembly: AssemblyFixture(typeof(Memorio.Scheduling.Tests.SchedulerCluster))]

namespace Memorio.Scheduling.Tests;

/// <summary>
/// The bootstrapped database with two worker schedulers on it, configured as the worker configures its
/// own (<see cref="MemorioScheduling.AddMemorioScheduling"/>, clustered) except for one-second retries:
/// nodes "a" and "b" share one store. The probe jobs write to the album schema with the album role, as
/// a module's job writes to its own schema.
/// </summary>
public sealed class SchedulerCluster : PostgresDatabase
{
    public static readonly TimeSpan[] TestRetryDelays = [TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)];

    private readonly List<IHost> _nodes = [];
    private NpgsqlDataSource? _album;

    internal ProbeStore Probes { get; private set; } = null!;

    internal IServiceProvider NodeA => _nodes[0].Services;

    internal IServiceProvider NodeB => _nodes[1].Services;

    protected override async Task OnBootstrappedAsync()
    {
        _album = NpgsqlDataSource.Create(ConnectionStringFor("memorio_album"));
        Probes = new ProbeStore(_album);
        await Probes.CreateTablesAsync();

        foreach (var node in new[] { "a", "b" })
        {
            var host = Build(node);
            await host.StartAsync();
            _nodes.Add(host);
        }
    }

    protected override async Task OnDisposingAsync()
    {
        foreach (var host in _nodes)
        {
            await host.StopAsync();
            host.Dispose();
        }

        if (_album is not null)
        {
            await _album.DisposeAsync();
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

    private IHost Build(string node)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration["Scheduling:ConnectionString"] = ConnectionStringFor(SchedulerRole);
        builder.Services.AddMemorioScheduling(builder.Configuration, TestRetryDelays);
        builder.Services.AddSingleton(Probes);
        builder.Services.AddSingleton(new ProbeNode(node));
        builder.Services.AddScheduledJob<RecordJob>(RecordJob.Name);
        builder.Services.AddScheduledJob<FailOnceJob>(FailOnceJob.Name);
        builder.Services.AddScheduledJob<TickJob>(TickJob.Name, TickJob.EverySecond);
        return builder.Build();
    }
}

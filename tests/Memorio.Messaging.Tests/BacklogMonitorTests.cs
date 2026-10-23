using System.Diagnostics.Metrics;
using Memorio.Hosting.Messaging;
using Memorio.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Npgsql;
using Wolverine;

namespace Memorio.Messaging.Tests;

/// <summary>
/// SD6 / QS12: the backlog metric shows a message stuck for more than 10 minutes. Runs against its own
/// database with the API side only, so nothing consumes the queue.
/// </summary>
public sealed class BacklogMonitorTests(BacklogMonitorTests.ApiOnly database) : IClassFixture<BacklogMonitorTests.ApiOnly>
{
    [Fact]
    public async Task A_message_waiting_in_the_queue_shows_its_age_and_the_count()
    {
        var time = new FakeTimeProvider(DateTimeOffset.UtcNow);
        using var metrics = new ServiceCollection().AddMetrics().BuildServiceProvider();
        var meterFactory = metrics.GetRequiredService<IMeterFactory>();
        await using var messaging = new MessagingDatabase(NpgsqlDataSource.Create(database.ConnectionStringFor(PostgresDatabase.MessagingRole)));
        var monitor = new BacklogMonitor(messaging, time, meterFactory, NullLogger<BacklogMonitor>.Instance);

        await database.PublishAsync(new ProbeRecorded(Guid.NewGuid(), time.GetUtcNow()));
        await monitor.SampleAsync(TestContext.Current.CancellationToken);
        time.Advance(TimeSpan.FromMinutes(11));
        await monitor.SampleAsync(TestContext.Current.CancellationToken);

        var gauges = Collect(meterFactory);
        Assert.Equal(TimeSpan.FromMinutes(11).TotalSeconds, gauges[("memorio.messaging.pending.oldest_age", "queue")]);
        Assert.Equal(1, gauges[("memorio.messaging.pending.count", "queue")]);
        Assert.Equal(0, gauges[("memorio.messaging.pending.oldest_age", "outbox")]);
        Assert.Equal(0, gauges[("memorio.messaging.pending.count", "inbox")]);
        Assert.Equal(0, gauges[("memorio.messaging.dead_letters.count", "")]);
    }

    private static Dictionary<(string Instrument, string Stage), double> Collect(IMeterFactory meterFactory)
    {
        var gauges = new Dictionary<(string, string), double>();
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == BacklogMonitor.MeterName && ReferenceEquals(instrument.Meter.Scope, meterFactory))
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<double>((instrument, value, tags, _) => gauges[(instrument.Name, StageOf(tags))] = value);
        listener.SetMeasurementEventCallback<long>((instrument, value, tags, _) => gauges[(instrument.Name, StageOf(tags))] = value);
        listener.Start();
        listener.RecordObservableInstruments();
        return gauges;
    }

    private static string StageOf(ReadOnlySpan<KeyValuePair<string, object?>> tags)
    {
        foreach (var tag in tags)
        {
            if (tag.Key == "stage")
            {
                return (string)tag.Value!;
            }
        }

        return "";
    }

    /// <summary>A database with only the API side running: events reach the queue and stay there.</summary>
    public sealed class ApiOnly : PostgresDatabase
    {
        private IHost? _api;

        public async Task PublishAsync(ProbeRecorded probe)
        {
            await using var scope = _api!.Services.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<IMessageBus>().PublishAsync(probe);

            // Published once the outbox has handed it to the queue.
            await using var connection = await OpenAsAsync(MessagingRole);
            Assert.True(await MessagingHosts.EventuallyAsync(
                async () =>
                {
                    await using var count = new NpgsqlCommand(
                        $"SELECT count(*) FROM {MemorioMessaging.QueueSchema}.wolverine_queue_{MemorioMessaging.Queue}",
                        connection);
                    return (long)(await count.ExecuteScalarAsync())! > 0;
                },
                TimeSpan.FromSeconds(30)));
        }

        protected override async Task OnBootstrappedAsync()
        {
            var builder = Host.CreateApplicationBuilder();
            builder.Configuration["Messaging:ConnectionString"] = ConnectionStringFor(MessagingRole);
            builder.Services.AddMemorioMessaging(builder.Configuration, MessagingHost.Api, []);
            _api = builder.Build();
            await _api.StartAsync();
        }

        protected override async Task OnDisposingAsync()
        {
            if (_api is not null)
            {
                await _api.StopAsync();
                _api.Dispose();
            }
        }
    }
}

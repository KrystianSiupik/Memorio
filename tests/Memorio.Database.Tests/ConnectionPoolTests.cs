using System.Diagnostics;
using System.Diagnostics.Metrics;
using Memorio.Hosting.Telemetry;
using Memorio.Testing;
using Npgsql;

namespace Memorio.Database.Tests;

/// <summary>
/// PLAT-21 (D-181, D-182): each role has its own bounded pool in a process, a bulkhead. When one module's
/// pool is used up (slow queries, a stuck transaction), its next request fails after the pool's timeout
/// with Npgsql's "pool exhausted" error and a timeout in the pool metrics, while another module still gets
/// a connection at once.
/// </summary>
public sealed class ConnectionPoolTests(BootstrappedDatabase database)
{
    private const int PoolSize = 2;
    private const int TimeoutSeconds = 1;

    [Fact]
    public async Task A_used_up_pool_of_one_module_neither_blocks_another_module_nor_hides_its_error()
    {
        var cancellation = TestContext.Current.CancellationToken;
        var timeouts = new Dictionary<string, int>();
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument is { Meter.Name: MemorioTelemetry.NpgsqlMeterName, Name: "db.client.connection.npgsql.timeouts" })
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<int>((_, value, tags, _) =>
        {
            foreach (var tag in tags)
            {
                if (tag is { Key: "db.client.connection.pool.name", Value: string pool })
                {
                    lock (timeouts)
                    {
                        timeouts[pool] = timeouts.GetValueOrDefault(pool) + value;
                    }
                }
            }
        });
        listener.Start();

        await using var album = Pool("album");
        await using var places = Pool("places");

        // The album module holds every connection of its pool.
        var held = new List<NpgsqlConnection>();
        for (var i = 0; i < PoolSize; i++)
        {
            held.Add(await album.OpenConnectionAsync(cancellation));
        }

        var waited = Stopwatch.StartNew();
        var exhausted = await Assert.ThrowsAsync<NpgsqlException>(() => album.OpenConnectionAsync(cancellation).AsTask());
        waited.Stop();
        Assert.Contains("connection pool has been exhausted", exhausted.Message, StringComparison.Ordinal);
        Assert.InRange(waited.Elapsed, TimeSpan.FromSeconds(TimeoutSeconds) * 0.9, TimeSpan.FromSeconds(TimeoutSeconds * 5));

        // Meanwhile the places module works as usual.
        var other = Stopwatch.StartNew();
        await using (var connection = await places.OpenConnectionAsync(cancellation))
        await using (var command = new NpgsqlCommand("SELECT 1", connection))
        {
            Assert.Equal(1, await command.ExecuteScalarAsync(cancellation));
        }

        other.Stop();
        Assert.True(other.Elapsed < TimeSpan.FromSeconds(TimeoutSeconds), $"places waited {other.Elapsed} for a connection");

        lock (timeouts)
        {
            Assert.Equal(1, timeouts.GetValueOrDefault("pool-test-album"));
            Assert.Equal(0, timeouts.GetValueOrDefault("pool-test-places"));
        }

        foreach (var connection in held)
        {
            await connection.DisposeAsync();
        }
    }

    private NpgsqlDataSource Pool(string schema) =>
        new NpgsqlDataSourceBuilder(database.ConnectionStringFor(ModuleSchemas.RoleOf(schema)))
        {
            ConnectionStringBuilder = { MaxPoolSize = PoolSize, Timeout = TimeoutSeconds },
            Name = "pool-test-" + schema,
        }.Build();
}

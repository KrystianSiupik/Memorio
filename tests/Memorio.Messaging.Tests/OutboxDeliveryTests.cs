using Memorio.Hosting.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Messaging.Tests;

/// <summary>
/// ADR 0006 end to end: an integration event stored in a module's transaction reaches the worker's
/// handlers through the PostgreSQL queue and the inbox, at least once and only when the transaction
/// commits (SD6, QS7).
/// </summary>
public sealed class OutboxDeliveryTests(MessagingHosts hosts)
{
    private static readonly TimeSpan DeliveryLimit = TimeSpan.FromMinutes(1);

    [Fact]
    public async Task An_event_stored_with_the_module_transaction_is_handled_by_the_worker_within_a_minute()
    {
        var probe = NewProbe();

        await StoreWithItemAsync(probe, Guid.NewGuid());

        Assert.True(await MessagingHosts.EventuallyAsync(() => Task.FromResult(Handled(probe) == 1), DeliveryLimit));
        Assert.True(await MessagingHosts.EventuallyAsync(async () => !await IsInOutboxAsync(probe), DeliveryLimit));
    }

    [Fact]
    public async Task An_event_of_a_failed_transaction_is_never_delivered()
    {
        var existing = Guid.NewGuid();
        await StoreWithItemAsync(NewProbe(), existing);
        var probe = NewProbe();

        await Assert.ThrowsAsync<DbUpdateException>(() => StoreWithItemAsync(probe, existing));

        await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        Assert.Equal(0, Handled(probe));
        Assert.False(await IsInOutboxAsync(probe));
    }

    [Fact]
    public async Task A_message_delivered_twice_is_handled_once()
    {
        var probe = NewProbe();
        await StoreWithItemAsync(probe, Guid.NewGuid());
        Assert.True(await MessagingHosts.EventuallyAsync(() => Task.FromResult(Handled(probe) == 1), DeliveryLimit));

        // The queue delivers the very same message (same id) again, as after a lost acknowledgement.
        await using var messaging = await hosts.OpenAsAsync(MessagingHosts.MessagingRole);
        await using (var redeliver = new NpgsqlCommand(
            $"""
            INSERT INTO {MemorioMessaging.QueueSchema}.wolverine_queue_{MemorioMessaging.Queue} (id, body, message_type)
            SELECT id, body, message_type FROM {MemorioMessaging.EnvelopeSchema}.wolverine_incoming_envelopes
            WHERE received_at LIKE 'postgresql://%' AND position(convert_to(@eventId, 'UTF8') in body) > 0
            """,
            messaging))
        {
            redeliver.Parameters.AddWithValue("eventId", probe.EventId.ToString());
            Assert.Equal(1, await redeliver.ExecuteNonQueryAsync(TestContext.Current.CancellationToken));
        }

        Assert.True(await MessagingHosts.EventuallyAsync(async () => await QueueLengthAsync(messaging) == 0, DeliveryLimit));
        await Task.Delay(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);
        Assert.Equal(1, Handled(probe));
    }

    [Fact]
    public async Task A_failing_module_is_retried_alone_and_another_module_handles_the_event_once()
    {
        var notified = new ModulesNotified(Guid.NewGuid(), DateTimeOffset.UtcNow);

        await using (var scope = hosts.Api.CreateAsyncScope())
        {
            await scope.ServiceProvider.GetRequiredService<IMessageBus>().PublishAsync(notified);
        }

        // Each module has its own queue (Separated): retrying the failing one does not run the healthy
        // one again, as it would if both handlers shared one message.
        Assert.True(await MessagingHosts.EventuallyAsync(
            () => Task.FromResult(hosts.Handled.Count(nameof(FailingModulesNotifiedHandler), notified.EventId) >= 2),
            DeliveryLimit));
        Assert.Equal(1, hosts.Handled.Count(nameof(ModulesNotifiedHandler), notified.EventId));
    }

    private static ProbeRecorded NewProbe() => new(Guid.NewGuid(), DateTimeOffset.UtcNow);

    private int Handled(ProbeRecorded probe) => hosts.Handled.Count(nameof(ProbeRecordedHandler), probe.EventId);

    /// <summary>One unit of work of the album module: a row in its schema and the event in the outbox.</summary>
    private async Task StoreWithItemAsync(ProbeRecorded probe, Guid itemId)
    {
        await using var scope = hosts.Api.CreateAsyncScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IDbContextOutbox<AlbumProbeContext>>();
        outbox.DbContext.Items.Add(new ProbeItem { Id = itemId });
        await outbox.PublishAsync(probe);
        await outbox.SaveChangesAndFlushMessagesAsync(TestContext.Current.CancellationToken);
    }

    private async Task<bool> IsInOutboxAsync(ProbeRecorded probe)
    {
        await using var messaging = await hosts.OpenAsAsync(MessagingHosts.MessagingRole);
        await using var command = new NpgsqlCommand(
            $"""
            SELECT count(*) FROM {MemorioMessaging.EnvelopeSchema}.wolverine_outgoing_envelopes
            WHERE position(convert_to(@eventId, 'UTF8') in body) > 0
            """,
            messaging);
        command.Parameters.AddWithValue("eventId", probe.EventId.ToString());
        return (long)(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken))! > 0;
    }

    private static async Task<long> QueueLengthAsync(NpgsqlConnection messaging)
    {
        await using var command = new NpgsqlCommand(
            $"SELECT count(*) FROM {MemorioMessaging.QueueSchema}.wolverine_queue_{MemorioMessaging.Queue}", messaging);
        return (long)(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken))!;
    }
}

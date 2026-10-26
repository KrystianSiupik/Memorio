using System.Globalization;
using Memorio.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Wolverine;

namespace Memorio.Messaging.Tests;

/// <summary>
/// MEM-E0-PLAT-20: an event whose handler failed every retry waits in the dead letters until someone
/// decides. Runs the SQL files of deploy/messaging/dead-letters.sh themselves, the way psql runs them
/// (<c>:'id'</c> is the id given to the script), so the script's statements are what is tested.
/// </summary>
public sealed class DeadLetterTests(MessagingHosts hosts)
{
    private static readonly TimeSpan Limit = TimeSpan.FromMinutes(1);

    [Fact]
    public async Task A_dead_letter_replayed_after_the_fix_is_handled_and_leaves_the_dead_letters()
    {
        var probe = await DeadLetterAsync();

        var listed = await RunScriptSqlAsync("list.sql");
        Assert.Contains(listed, row => row.Contains(nameof(ReplayProbe), StringComparison.Ordinal));
        var shown = Assert.Single(await RunScriptSqlAsync("show.sql", probe.DeadLetterId));
        Assert.Contains(ReplayProbeHandler.Failure, shown, StringComparison.Ordinal);
        Assert.Contains(probe.Event.EventId.ToString(), shown, StringComparison.Ordinal);

        hosts.Handled.Repair(probe.Event.EventId);
        Assert.Equal(probe.DeadLetterId, Assert.Single(await RunScriptSqlAsync("replay.sql", probe.DeadLetterId)));

        // The worker's durability agent moves it back to the inbox and recovers it to the listener (D-185).
        Assert.True(await MessagingHosts.EventuallyAsync(
            () => Task.FromResult(hosts.Handled.Count(nameof(ReplayProbeHandler), probe.Event.EventId) == 1),
            Limit));
        Assert.True(await MessagingHosts.EventuallyAsync(async () => await DeadLettersOfAsync(probe.Handler) == 0, Limit));
    }

    [Fact]
    public async Task A_discarded_dead_letter_is_gone_and_never_handled()
    {
        var probe = await DeadLetterAsync();

        var discarded = Assert.Single(await RunScriptSqlAsync("discard.sql", probe.DeadLetterId));
        Assert.Contains(probe.DeadLetterId, discarded, StringComparison.Ordinal);
        Assert.Contains(probe.Handler, discarded, StringComparison.Ordinal);

        Assert.Empty(await RunScriptSqlAsync("show.sql", probe.DeadLetterId));
        Assert.Equal(0, hosts.Handled.Count(nameof(ReplayProbeHandler), probe.Event.EventId));
    }

    /// <summary>Publishes an event whose handler is broken and waits until it fails every retry.</summary>
    private async Task<(ReplayProbe Event, string DeadLetterId, string Handler)> DeadLetterAsync()
    {
        var probe = new ReplayProbe(Guid.NewGuid(), DateTimeOffset.UtcNow);
        hosts.Handled.Break(probe.EventId);
        await using (var scope = hosts.Api.CreateAsyncScope())
        {
            await scope.ServiceProvider.GetRequiredService<IMessageBus>().PublishAsync(probe);
        }

        (string Id, string Handler)? deadLetter = null;
        Assert.True(await MessagingHosts.EventuallyAsync(
            async () => (deadLetter = await FindDeadLetterAsync(probe.EventId)) is not null,
            Limit));
        return (probe, deadLetter!.Value.Id, deadLetter.Value.Handler);
    }

    private async Task<(string Id, string Handler)?> FindDeadLetterAsync(Guid eventId)
    {
        await using var connection = await hosts.OpenAsAsync(PostgresDatabase.MessagingRole);
        await using var command = new NpgsqlCommand(
            "SELECT id::text, received_at FROM wolverine.wolverine_dead_letters WHERE encode(body, 'escape') LIKE @event",
            connection);
        command.Parameters.AddWithValue("event", $"%{eventId}%");
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);
        return await reader.ReadAsync(TestContext.Current.CancellationToken) ? (reader.GetString(0), reader.GetString(1)) : null;
    }

    private async Task<long> DeadLettersOfAsync(string handler)
    {
        await using var connection = await hosts.OpenAsAsync(PostgresDatabase.MessagingRole);
        await using var command = new NpgsqlCommand(
            "SELECT count(*) FROM wolverine.wolverine_dead_letters WHERE received_at = @handler",
            connection);
        command.Parameters.AddWithValue("handler", handler);
        return (long)(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken))!;
    }

    /// <summary>
    /// A SQL file of deploy/messaging as the owner's psql session would run it (it switches to the role
    /// itself), with the script's id in place of <c>:'id'</c>; every row of every result, columns joined by "|".
    /// </summary>
    private async Task<List<string>> RunScriptSqlAsync(string file, string? id = null)
    {
        var sql = await File.ReadAllTextAsync(
            Path.Combine(PostgresDatabase.RepositoryRoot(), "deploy", "messaging", file),
            TestContext.Current.CancellationToken);
        await using var connection = await hosts.OpenAsOwnerAsync();
        await using var command = new NpgsqlCommand(sql.Replace(":'id'", "@id", StringComparison.Ordinal), connection);
        command.Parameters.AddWithValue("id", id ?? "");
        var rows = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);
        do
        {
            while (await reader.ReadAsync(TestContext.Current.CancellationToken))
            {
                rows.Add(string.Join('|', Enumerable.Range(0, reader.FieldCount).Select(i => reader.IsDBNull(i) ? "" : Convert.ToString(reader.GetValue(i), CultureInfo.InvariantCulture))));
            }
        }
        while (await reader.NextResultAsync(TestContext.Current.CancellationToken));

        return rows;
    }
}

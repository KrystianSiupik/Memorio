using Memorio.SharedKernel.Scheduling;
using Npgsql;

namespace Memorio.Scheduling.Tests;

internal sealed record ProbeNode(string Name);

/// <summary>
/// What the probe jobs did, in the album schema: <c>job_runs</c> logs every firing (it grows with every
/// run, so tests see how often a job ran and on which node), <c>job_effects</c> holds the jobs' effect,
/// which an idempotent job writes once per key whatever the number of runs.
/// </summary>
internal sealed class ProbeStore(NpgsqlDataSource album)
{
    public async Task CreateTablesAsync()
    {
        await ExecuteAsync(
            """
            CREATE TABLE job_runs (id bigserial PRIMARY KEY, job text NOT NULL, key text, scheduled_for timestamptz NOT NULL, attempt int NOT NULL, node text NOT NULL);
            CREATE TABLE job_effects (key text PRIMARY KEY, job text NOT NULL, value text, created_by_attempt int NOT NULL);
            """);
    }

    public Task LogRunAsync(ScheduledJobRun run, ProbeNode node, CancellationToken cancellationToken) =>
        ExecuteAsync(
            "INSERT INTO job_runs (job, key, scheduled_for, attempt, node) VALUES (@job, @key, @scheduled_for, @attempt, @node)",
            cancellationToken,
            ("job", run.Job),
            ("key", (object?)run.Key ?? DBNull.Value),
            ("scheduled_for", run.ScheduledFor),
            ("attempt", run.Attempt),
            ("node", node.Name));

    /// <summary>The idempotent effect: the first run for a key writes it, every later one changes nothing.</summary>
    public Task RecordEffectAsync(string key, ScheduledJobRun run, CancellationToken cancellationToken) =>
        ExecuteAsync(
            "INSERT INTO job_effects (key, job, value, created_by_attempt) VALUES (@key, @job, @value, @attempt) ON CONFLICT (key) DO NOTHING",
            cancellationToken,
            ("key", key),
            ("job", run.Job),
            ("value", (object?)run.Data.GetValueOrDefault("value") ?? DBNull.Value),
            ("attempt", run.Attempt));

    public async Task<IReadOnlyList<(string? Key, DateTimeOffset ScheduledFor, int Attempt, string Node)>> RunsAsync(string job)
    {
        await using var command = album.CreateCommand(
            "SELECT key, scheduled_for, attempt, node FROM job_runs WHERE job = @job ORDER BY id");
        command.Parameters.AddWithValue("job", job);
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);
        var runs = new List<(string?, DateTimeOffset, int, string)>();
        while (await reader.ReadAsync(TestContext.Current.CancellationToken))
        {
            runs.Add((
                reader.IsDBNull(0) ? null : reader.GetString(0),
                reader.GetFieldValue<DateTimeOffset>(1),
                reader.GetInt32(2),
                reader.GetString(3)));
        }

        return runs;
    }

    public async Task<IReadOnlyList<(string Key, string? Value, int CreatedByAttempt)>> EffectsAsync(string keyPrefix)
    {
        await using var command = album.CreateCommand(
            "SELECT key, value, created_by_attempt FROM job_effects WHERE starts_with(key, @prefix) ORDER BY key");
        command.Parameters.AddWithValue("prefix", keyPrefix);
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);
        var effects = new List<(string, string?, int)>();
        while (await reader.ReadAsync(TestContext.Current.CancellationToken))
        {
            effects.Add((reader.GetString(0), reader.IsDBNull(1) ? null : reader.GetString(1), reader.GetInt32(2)));
        }

        return effects;
    }

    private async Task ExecuteAsync(string sql, CancellationToken cancellationToken = default, params (string Name, object Value)[] parameters)
    {
        await using var command = album.CreateCommand(sql);
        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}

/// <summary>A deadline job: logs the firing and records its effect for the deadline's key once.</summary>
internal sealed class RecordJob(ProbeStore store, ProbeNode node) : IScheduledJob
{
    public const string Name = "Probe.Record";

    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        await store.LogRunAsync(run, node, cancellationToken);
        await store.RecordEffectAsync(run.Key!, run, cancellationToken);
    }
}

/// <summary>Fails its first attempt, then records its effect: proves the retry.</summary>
internal sealed class FailOnceJob(ProbeStore store, ProbeNode node) : IScheduledJob
{
    public const string Name = "Probe.FailOnce";

    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        await store.LogRunAsync(run, node, cancellationToken);
        if (run.Attempt == 1)
        {
            throw new InvalidOperationException("The first attempt fails on purpose.");
        }

        await store.RecordEffectAsync(run.Key!, run, cancellationToken);
    }
}

/// <summary>A recurring job firing every second; its effect is keyed by the occurrence it ran for.</summary>
internal sealed class TickJob(ProbeStore store, ProbeNode node) : IScheduledJob
{
    public const string Name = "Probe.Tick";
    public const string EverySecond = "* * * ? * *";

    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        await store.LogRunAsync(run, node, cancellationToken);
        await store.RecordEffectAsync($"tick:{run.ScheduledFor:O}", run, cancellationToken);
    }
}

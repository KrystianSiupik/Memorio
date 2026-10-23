using System.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Memorio.Hosting.Messaging;

/// <summary>
/// Backlog metrics of the worker (SD6, QS12): age of the oldest pending message and number of pending
/// messages per stage (outbox: stored, not yet sent; queue: sent, not yet received; inbox: received,
/// not yet handled) and the number of dead letters. Alert when an age passes 10 minutes or a dead
/// letter appears (rules and export: MEM-E0-PLAT-10).
/// </summary>
internal sealed partial class BacklogMonitor : BackgroundService
{
    public const string MeterName = "Memorio.Messaging";
    public static readonly TimeSpan SamplingInterval = TimeSpan.FromSeconds(15);

    private static readonly (string Stage, string Sql)[] Stages =
    [
        ("outbox", $"SELECT id FROM {MemorioMessaging.EnvelopeSchema}.wolverine_outgoing_envelopes"),
        ("queue", $"SELECT id FROM {MemorioMessaging.QueueSchema}.wolverine_queue_{MemorioMessaging.Queue}"),
        ("inbox", $"""
            SELECT id FROM {MemorioMessaging.EnvelopeSchema}.wolverine_incoming_envelopes
            WHERE status = 'Incoming' OR (status = 'Scheduled' AND execution_time <= now())
            """),
    ];

    private const string DeadLettersSql = $"SELECT count(*) FROM {MemorioMessaging.EnvelopeSchema}.wolverine_dead_letters";

    private readonly MessagingDatabase _database;
    private readonly TimeProvider _time;
    private readonly ILogger<BacklogMonitor> _logger;
    private readonly Dictionary<string, BacklogAges> _ages = Stages.ToDictionary(stage => stage.Stage, _ => new BacklogAges());
    private readonly Lock _lock = new();
    private long _deadLetters;

    public BacklogMonitor(MessagingDatabase database, TimeProvider time, IMeterFactory meterFactory, ILogger<BacklogMonitor> logger)
    {
        _database = database;
        _time = time;
        _logger = logger;

        var meter = meterFactory.Create(MeterName);
        meter.CreateObservableGauge(
            "memorio.messaging.pending.oldest_age",
            () => Measure(ages => ages.OldestAge(_time.GetUtcNow()).TotalSeconds),
            unit: "s",
            description: "Age of the oldest pending message per stage (outbox, queue, inbox).");
        meter.CreateObservableGauge(
            "memorio.messaging.pending.count",
            () => Measure(ages => (double)ages.Count),
            unit: "{message}",
            description: "Pending messages per stage (outbox, queue, inbox).");
        meter.CreateObservableGauge(
            "memorio.messaging.dead_letters.count",
            () => Interlocked.Read(ref _deadLetters),
            unit: "{message}",
            description: "Messages that failed every retry and wait for a person.");
    }

    /// <summary>Takes one sample of every stage; the background loop calls it every <see cref="SamplingInterval"/>.</summary>
    public async Task SampleAsync(CancellationToken cancellationToken)
    {
        var now = _time.GetUtcNow();
        foreach (var (stage, sql) in Stages)
        {
            var ids = new List<Guid>();
            await using (var command = _database.DataSource.CreateCommand(sql))
            await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    ids.Add(reader.GetGuid(0));
                }
            }

            lock (_lock)
            {
                _ages[stage].Observe(ids, now);
            }
        }

        await using var deadLetters = _database.DataSource.CreateCommand(DeadLettersSql);
        Interlocked.Exchange(ref _deadLetters, (long)(await deadLetters.ExecuteScalarAsync(cancellationToken))!);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(SamplingInterval, _time);
        do
        {
            try
            {
                await SampleAsync(stoppingToken);
            }
            catch (NpgsqlException exception)
            {
                // Database unavailable: /ready reports it; the gauges keep their last values.
                SamplingFailed(_logger, exception);
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private List<Measurement<double>> Measure(Func<BacklogAges, double> value)
    {
        lock (_lock)
        {
            return _ages.Select(stage => new Measurement<double>(value(stage.Value), new KeyValuePair<string, object?>("stage", stage.Key))).ToList();
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Sampling the messaging backlog failed")]
    private static partial void SamplingFailed(ILogger logger, Exception exception);
}

using System.Collections.Concurrent;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Archive.Tests;

/// <summary>Holds deadlines the way the scheduler does: one per job and key, a later one replaces it.</summary>
public sealed class FakeDeadlines : IDeadlines
{
    private readonly ConcurrentDictionary<(string Job, string Key), ScheduledJobRun> _deadlines = new();

    public ScheduledJobRun? Find(string job, string key) => _deadlines.GetValueOrDefault((job, key));

    public Task ScheduleAsync(string job, string key, DateTimeOffset dueAt, IReadOnlyDictionary<string, string>? data, CancellationToken cancellationToken)
    {
        _deadlines[(job, key)] = new ScheduledJobRun(job, key, dueAt, data ?? new Dictionary<string, string>(), Attempt: 1);
        return Task.CompletedTask;
    }

    public Task CancelAsync(string job, string key, CancellationToken cancellationToken)
    {
        _deadlines.TryRemove((job, key), out _);
        return Task.CompletedTask;
    }
}

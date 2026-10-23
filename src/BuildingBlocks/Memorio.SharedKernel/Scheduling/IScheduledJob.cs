namespace Memorio.SharedKernel.Scheduling;

/// <summary>
/// Work a module runs in the worker on a schedule or at a deadline (ADR 0012, D-036): a step of a long
/// process that calls the module's aggregates and never changes data around them. The scheduler may run
/// one firing more than once (a retry after a failure, a node that died mid-run, the same deadline
/// scheduled again), so a job is idempotent: running it twice leaves the state it leaves after one run
/// (SD12). Throwing fails the firing, which is retried after <see cref="ScheduledJobRules.RetryDelays"/>.
/// </summary>
public interface IScheduledJob
{
    Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken);
}

/// <summary>One firing of a job.</summary>
/// <param name="Job">The job's name ("Module.Job").</param>
/// <param name="Key">The deadline's key (e.g. the id of the aggregate it is about); null for a recurring run.</param>
/// <param name="ScheduledFor">When the firing was due; the job decides with this time, not with "now".</param>
/// <param name="Data">What the deadline was scheduled with; empty for a recurring run.</param>
/// <param name="Attempt">1 on the first attempt, higher on retries.</param>
public sealed record ScheduledJobRun(
    string Job,
    string? Key,
    DateTimeOffset ScheduledFor,
    IReadOnlyDictionary<string, string> Data,
    int Attempt);

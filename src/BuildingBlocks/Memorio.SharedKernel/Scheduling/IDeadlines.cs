namespace Memorio.SharedKernel.Scheduling;

/// <summary>
/// Port to the worker's scheduler (ADR 0012): a module schedules a deadline of one of its jobs, e.g. the
/// end of a 30-day window of one account. Available in the worker only; a deadline that follows a change
/// in the API is scheduled by the worker's handler of the event that change stored in the outbox, so it
/// is never lost when the change commits (D-157). Both operations are idempotent.
/// </summary>
public interface IDeadlines
{
    /// <summary>
    /// Runs <paramref name="job"/> for <paramref name="key"/> at <paramref name="dueAt"/> (at once when it
    /// is past). Scheduling the same job and key again replaces the deadline, its time and its data.
    /// </summary>
    Task ScheduleAsync(
        string job,
        string key,
        DateTimeOffset dueAt,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken);

    /// <summary>Cancels the deadline; cancelling one that does not exist (or already ran) succeeds.</summary>
    Task CancelAsync(string job, string key, CancellationToken cancellationToken);
}

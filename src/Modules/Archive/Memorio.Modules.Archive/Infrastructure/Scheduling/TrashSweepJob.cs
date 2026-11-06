using Memorio.Modules.Archive.Application.Commands.DeleteMemory;
using Memorio.Modules.Archive.Application.Policies;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Memorio.Modules.Archive.Infrastructure.Scheduling;

/// <summary>
/// Safety net of the promise that the trash lasts 30 days (D-228, as D-215 for accounts): once a day, what is
/// still in the trash after its window goes the way its deadline would have, and what a legal hold kept goes once it
/// ended (D-312). Each memory has its own scope,
/// and one failing memory does not stop the others.
/// </summary>
internal sealed partial class TrashSweepJob(IMemories memories, IServiceScopeFactory scopes, ILogger<TrashSweepJob> logger) : IScheduledJob
{
    public const string JobName = "Archive.TrashSweep";

    /// <summary>Every day at 03:30 UTC.</summary>
    public const string Cron = "0 30 3 * * ?";

    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        var failed = 0;
        foreach (var overdue in await memories.FindOverdueAsync(run.ScheduledFor, cancellationToken))
        {
            LogOverdue(logger, overdue.MemoryId.Value, overdue.Sequence);
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<DeleteMemoryWhenTrashWindowPassed>().HandleAsync(
                    new TrashWindowPassed(overdue.MemoryId, overdue.Sequence, run.ScheduledFor),
                    cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                LogFailed(logger, exception, overdue.MemoryId.Value);
                failed++;
            }
        }

        foreach (var waiting in await memories.FindAwaitingErasureAsync(cancellationToken))
        {
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<EraseAwaitingHandler>().HandleAsync(waiting, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                LogFailed(logger, exception, waiting.Value);
                failed++;
            }
        }

        if (failed > 0)
        {
            // Fails the firing, so the scheduler retries and the given_up metric sees what never succeeded.
            throw new InvalidOperationException($"{failed} overdue memories failed.");
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "The trash window of memory {MemoryId} (sequence {Sequence}) is overdue; its deadline did not delete it")]
    private static partial void LogOverdue(ILogger logger, Guid memoryId, int sequence);

    [LoggerMessage(Level = LogLevel.Error, Message = "Deleting the overdue memory {MemoryId} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, Guid memoryId);
}

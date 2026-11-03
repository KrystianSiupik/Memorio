using Memorio.Modules.Account.Application.Policies;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Memorio.Modules.Account.Infrastructure.Scheduling;

/// <summary>
/// Safety net of the promise to delete after 30 days (D-215): once a day, a window still open after its end
/// goes the way its deadline would have, whatever happened to the deadline (given up after its retries,
/// its event in the dead letters). Each account has its own scope: a save that lost leaves its event in
/// the outbox of that scope, and one failing account does not stop the others.
/// </summary>
internal sealed partial class AccountDeletionSweepJob(
    IAccountDeletions deletions,
    IServiceScopeFactory scopes,
    ILogger<AccountDeletionSweepJob> logger) : IScheduledJob
{
    public const string JobName = "Account.DeletionSweep";

    /// <summary>Every day at 03:00 UTC.</summary>
    public const string Cron = "0 0 3 * * ?";

    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        var failed = 0;
        foreach (var overdue in await deletions.FindOverdueAsync(run.ScheduledFor, cancellationToken))
        {
            LogOverdue(logger, overdue.AccountId.Value, overdue.Sequence);
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<DeleteAccountWhenWindowPassed>().HandleAsync(
                    new DeletionWindowPassed(overdue.AccountId, overdue.Sequence, run.ScheduledFor),
                    cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                LogFailed(logger, exception, overdue.AccountId.Value);
                failed++;
            }
        }

        if (failed > 0)
        {
            // Fails the firing, so the scheduler retries and the given_up metric sees what never succeeded.
            throw new InvalidOperationException($"{failed} overdue account deletions failed.");
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "The deletion window of account {AccountId} (sequence {Sequence}) is overdue; its deadline did not delete it")]
    private static partial void LogOverdue(ILogger logger, Guid accountId, int sequence);

    [LoggerMessage(Level = LogLevel.Error, Message = "Deleting the overdue account {AccountId} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, Guid accountId);
}

using Memorio.Modules.Succession.Application.Commands.Cases;
using Memorio.Modules.Succession.Domain.Cases;
using Memorio.SharedKernel.Scheduling;
using Memorio.SharedKernel.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Memorio.Modules.Succession.Infrastructure.Scheduling;

/// <summary>P38: the deadline of an offer firing is E164; decides with the time it was due (ADR 0012).</summary>
internal sealed class AnswerDeadlineJob(ExpireOfferHandler expire) : IScheduledJob
{
    public Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken) =>
        expire.HandleAsync(new TakeoverCaseId(Guid.Parse(run.Key!)), run.ScheduledFor, cancellationToken);
}

/// <summary>
/// What the service decided through deploy/succession/cases.sh, every minute (D-295): no public way in to anything about a
/// death, and every decision goes through the same handler. Each has its own scope, and one failing does not stop others.
/// </summary>
internal sealed partial class OperatorDecisionsJob(ITakeoverCases cases, IServiceScopeFactory scopes, TimeProvider clock, ILogger<OperatorDecisionsJob> logger) : IScheduledJob
{
    public const string JobName = "Succession.OperatorDecisions";

    public const string Cron = "0 * * * * ?";

    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        var failed = 0;
        foreach (var id in await cases.FindPendingDecisionsAsync(cancellationToken))
        {
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                var own = scope.ServiceProvider.GetRequiredService<ITakeoverCases>();
                var decision = await own.FindDecisionAsync(id, cancellationToken);
                if (decision?.AppliedAt is not null || decision is null)
                {
                    continue;
                }

                var decided = await scope.ServiceProvider.GetRequiredService<DecideReportHandler>().HandleAsync(
                    new DecideReport(new TakeoverCaseId(decision.CaseId), decision.Confirmed),
                    cancellationToken);
                decision.MarkApplied(decided.IsSuccess ? "Applied" : decided.Error.Code, clock.GetUtcNow());
                if (!await own.SaveAsync(cancellationToken))
                {
                    throw new InvalidOperationException("Another run got there first.");
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                LogFailed(logger, exception, id);
                failed++;
            }
        }

        if (failed > 0)
        {
            throw new InvalidOperationException($"{failed} decisions failed.");
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Applying the service's decision {DecisionId} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, Guid decisionId);
}

/// <summary>
/// Once a day: an offer whose deadline got lost ends anyway (as D-215), and a certificate goes 12 months after its case
/// closed (D-055). Each in its own scope, and one failing does not stop others.
/// </summary>
internal sealed partial class SuccessionSweepJob(ITakeoverCases cases, IServiceScopeFactory scopes, ILogger<SuccessionSweepJob> logger) : IScheduledJob
{
    public const string JobName = "Succession.Sweep";

    /// <summary>Every day at 03:45 UTC.</summary>
    public const string Cron = "0 45 3 * * ?";

    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        var failed = 0;
        foreach (var id in await cases.FindOverdueAsync(run.ScheduledFor, cancellationToken))
        {
            failed += await TryAsync(id, (scope, at) => scope.GetRequiredService<ExpireOfferHandler>().HandleAsync(id, at, cancellationToken), run.ScheduledFor, cancellationToken);
        }

        foreach (var id in await cases.FindExpiredCertificatesAsync(run.ScheduledFor.AddMonths(-12), cancellationToken))
        {
            failed += await TryAsync(id, (scope, _) => ForgetAsync(scope, id, cancellationToken), run.ScheduledFor, cancellationToken);
        }

        if (failed > 0)
        {
            throw new InvalidOperationException($"{failed} cases failed.");
        }
    }

    private static async Task ForgetAsync(IServiceProvider services, TakeoverCaseId id, CancellationToken cancellationToken)
    {
        var own = services.GetRequiredService<ITakeoverCases>();
        if (await own.FindAsync(id, cancellationToken) is not { CertificateKey: { } key } found)
        {
            return;
        }

        await services.GetRequiredService<IObjectStorage>().DeleteAsync(new StorageKey(key), cancellationToken);
        found.ForgetCertificate();
        if (!await own.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this case got there first; the retry reads it again.");
        }
    }

    private async Task<int> TryAsync(TakeoverCaseId id, Func<IServiceProvider, DateTimeOffset, Task> step, DateTimeOffset at, CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopes.CreateAsyncScope();
            await step(scope.ServiceProvider, at);
            return 0;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            LogFailed(logger, exception, id.Value);
            return 1;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "The daily step of case {CaseId} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, Guid caseId);
}

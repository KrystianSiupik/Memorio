using Memorio.Modules.Plans.Application.Commands.Cycle;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Memorio.Modules.Plans.Infrastructure.Scheduling;

/// <summary>
/// Moves every cycle that has something due, with the time the firing was due (D-280): one hourly step instead of
/// a deadline per plan, so there is nothing to cancel when someone pays. Each cycle has its own scope, and one
/// failing does not stop the others.
/// </summary>
internal sealed partial class PlanCycleJob(IPlanCycles cycles, IServiceScopeFactory scopes, ILogger<PlanCycleJob> logger) : IScheduledJob
{
    public const string JobName = "Plans.Cycle";

    /// <summary>Every hour at minute 15, UTC.</summary>
    public const string Cron = "0 15 * * * ?";

    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        var failed = 0;
        foreach (var resource in await cycles.FindDueAsync(run.ScheduledFor, cancellationToken))
        {
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                var advanced = await scope.ServiceProvider.GetRequiredService<AdvanceCycleHandler>().HandleAsync(resource, run.ScheduledFor, cancellationToken);
                if (advanced.IsFailure)
                {
                    throw new InvalidOperationException(advanced.Error.Code);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                LogFailed(logger, exception, resource.Kind.ToString(), resource.Id);
                failed++;
            }
        }

        if (failed > 0)
        {
            // Fails the firing, so the scheduler retries and the given_up metric sees what never succeeded.
            throw new InvalidOperationException($"{failed} plan cycles failed.");
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "The step of the plan cycle of {Kind} {ResourceId} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, string kind, Guid resourceId);
}

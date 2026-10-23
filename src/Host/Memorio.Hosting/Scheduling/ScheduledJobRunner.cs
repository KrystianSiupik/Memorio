using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Memorio.Hosting.Scheduling;

/// <summary>
/// The one Quartz job type: runs the module job named by the Quartz job's name, in the scope Quartz
/// created for this firing. The store holds only job names, never module types, so a module can change
/// its inside without breaking what is already scheduled. One firing of a Quartz job at a time across the
/// cluster; a failure is rethrown for Quartz to retry under the default policy.
/// </summary>
[DisallowConcurrentExecution]
internal sealed partial class ScheduledJobRunner(
    IServiceProvider services,
    SchedulingMetrics metrics,
    ILogger<ScheduledJobRunner> logger) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        var name = context.JobDetail.Key.Name;
        var job = services.GetKeyedService<IScheduledJob>(name);
        if (job is null)
        {
            // Retrying cannot help: the code that ran this job is gone. The firing stays visible in the log.
            LogUnknownJob(logger, name);
            metrics.Record(name, SchedulingMetrics.Unknown);
            return;
        }

        var deadline = context.JobDetail.Key.Group == MemorioScheduling.DeadlinesGroup;
        var data = context.Trigger.JobDataMap
            .Where(entry => entry.Value is string)
            .ToDictionary(entry => entry.Key, entry => (string)entry.Value!, StringComparer.Ordinal);
        var run = new ScheduledJobRun(
            name,
            deadline ? context.Trigger.Key.Name : null,
            context.ScheduledFireTimeUtc ?? context.FireTimeUtc,
            data,
            context.RetryAttempt + 1);

        try
        {
            await job.RunAsync(run, cancellationToken);
            metrics.Record(name, SchedulingMetrics.Succeeded);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            var retries = context.RetryPolicy?.MaxAttempts ?? 0;
            if (context.RetryAttempt < retries)
            {
                LogFailedWillRetry(logger, exception, name, run.Key, run.Attempt);
                metrics.Record(name, SchedulingMetrics.Failed);
            }
            else
            {
                LogGivenUp(logger, exception, name, run.Key, run.Attempt);
                metrics.Record(name, SchedulingMetrics.GivenUp);
            }

            throw;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Scheduled job {Job} is not registered by any module; the firing is dropped.")]
    private static partial void LogUnknownJob(ILogger logger, string job);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Scheduled job {Job} (key {Key}) failed on attempt {Attempt}; it will be retried.")]
    private static partial void LogFailedWillRetry(ILogger logger, Exception exception, string job, string? key, int attempt);

    [LoggerMessage(Level = LogLevel.Error, Message = "Scheduled job {Job} (key {Key}) failed on attempt {Attempt}, the last one; it is given up.")]
    private static partial void LogGivenUp(ILogger logger, Exception exception, string job, string? key, int attempt);
}

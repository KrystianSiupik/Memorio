using Memorio.Modularity;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Memorio.Hosting.Scheduling;

/// <summary>
/// Brings the stored recurring jobs in line with the registered ones at worker start: each job with a
/// cron schedule is stored (replaced, so a changed schedule takes effect) in <see cref="MemorioScheduling.RecurringGroup"/>,
/// and a stored one no module registers any more is removed. Idempotent, so every node of a cluster may
/// do it. An invalid cron expression stops the worker at start.
/// </summary>
internal sealed class RecurringJobs(ISchedulerFactory schedulers, IEnumerable<ScheduledJobDefinition> definitions) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var recurring = definitions.Where(definition => definition.Cron is not null).ToList();
        foreach (var definition in recurring.Where(definition => !CronExpression.TryParse(definition.Cron, out _)))
        {
            throw new InvalidOperationException($"Scheduled job {definition.Name} has an invalid cron expression \"{definition.Cron}\".");
        }

        var scheduler = await schedulers.GetScheduler(cancellationToken);
        foreach (var definition in recurring)
        {
            var job = JobBuilder.Create<ScheduledJobRunner>()
                .WithIdentity(definition.Name, MemorioScheduling.RecurringGroup)
                .StoreDurably()
                .RequestRecovery()
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity(definition.Name, MemorioScheduling.RecurringGroup)
                .ForJob(job)
                .WithCronSchedule(
                    definition.Cron!,
                    cron => cron.InTimeZone(TimeZoneInfo.Utc).WithMisfireInstruction(CronTriggerMisfireInstruction.FireAndProceed))
                .Build();

            await scheduler.ScheduleJob(job, trigger, ScheduleJobOptions.Replacing, cancellationToken);
        }

        var registered = recurring.Select(definition => definition.Name).ToHashSet(StringComparer.Ordinal);
        var stored = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(MemorioScheduling.RecurringGroup), cancellationToken);
        foreach (var stale in stored.Where(key => !registered.Contains(key.Name)))
        {
            await scheduler.DeleteJob(stale, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

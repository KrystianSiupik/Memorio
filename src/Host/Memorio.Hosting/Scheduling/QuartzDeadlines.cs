using Memorio.SharedKernel.Scheduling;
using Quartz;

namespace Memorio.Hosting.Scheduling;

/// <summary>
/// <see cref="IDeadlines"/> on Quartz: one durable Quartz job per job name in <see cref="MemorioScheduling.DeadlinesGroup"/>,
/// one trigger per deadline keyed (key, job name), so scheduling again replaces and cancelling is one delete.
/// A deadline missed while no worker ran fires as soon as one runs.
/// </summary>
internal sealed class QuartzDeadlines(ISchedulerFactory schedulers) : IDeadlines
{
    public async Task ScheduleAsync(
        string job,
        string key,
        DateTimeOffset dueAt,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken)
    {
        Validate(job, key);
        var scheduler = await schedulers.GetScheduler(cancellationToken);

        var jobDetail = JobBuilder.Create<ScheduledJobRunner>()
            .WithIdentity(JobKeyOf(job))
            .StoreDurably()
            .RequestRecovery()
            .Build();
        var triggerData = new JobDataMap();
        foreach (var (name, value) in data ?? new Dictionary<string, string>())
        {
            triggerData.Add(name, value);
        }

        var trigger = TriggerBuilder.Create()
            .WithIdentity(TriggerKeyOf(job, key))
            .ForJob(jobDetail)
            .UsingJobData(triggerData)
            .StartAt(dueAt)
            .WithSimpleSchedule(schedule => schedule.WithMisfireInstruction(SimpleTriggerMisfireInstruction.FireNow))
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger, ScheduleJobOptions.Replacing, cancellationToken);
    }

    public async Task CancelAsync(string job, string key, CancellationToken cancellationToken)
    {
        Validate(job, key);
        var scheduler = await schedulers.GetScheduler(cancellationToken);
        await scheduler.UnscheduleJob(TriggerKeyOf(job, key), cancellationToken);
    }

    internal static JobKey JobKeyOf(string job) => new(job, MemorioScheduling.DeadlinesGroup);

    internal static TriggerKey TriggerKeyOf(string job, string key) => new(key, job);

    private static void Validate(string job, string key)
    {
        if (!ScheduledJobRules.IsValidName(job))
        {
            throw new ArgumentException($"A scheduled job is named \"Module.Job\", not \"{job}\".", nameof(job));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(key);
    }
}

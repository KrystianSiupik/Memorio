using Memorio.Hosting.Scheduling;
using Memorio.Modularity;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Memorio.Scheduling.Tests;

/// <summary>
/// MEM-E0-PLAT-08 on PostgreSQL with two clustered nodes: deadlines, retries, recurring jobs, and the
/// fitness function SD12 (a firing run twice leaves the state of one run). Needs Docker.
/// </summary>
public sealed class SchedulerTests(SchedulerCluster cluster)
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    private ProbeStore Probes => cluster.Probes;

    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task SD12_a_deadline_run_twice_leaves_the_state_of_one_run()
    {
        var key = NewKey("twice");
        var deadlines = cluster.NodeA.GetRequiredService<IDeadlines>();
        var data = new Dictionary<string, string> { ["value"] = "first" };

        await deadlines.ScheduleAsync(RecordJob.Name, key, DateTimeOffset.UtcNow, data, Cancellation);
        Assert.True(await RunsReachAsync(RecordJob.Name, key, 1));
        var afterOneRun = await Probes.EffectsAsync(key);

        // The same deadline again, as a retry, a recovered firing or a repeated schedule would run it.
        await deadlines.ScheduleAsync(RecordJob.Name, key, DateTimeOffset.UtcNow, data, Cancellation);
        Assert.True(await RunsReachAsync(RecordJob.Name, key, 2));

        Assert.Equal(afterOneRun, await Probes.EffectsAsync(key));
        Assert.Equal([(key, "first", 1)], afterOneRun);
    }

    [Fact]
    public async Task Every_deadline_runs_exactly_once_across_the_cluster()
    {
        var keys = Enumerable.Range(0, 20).Select(_ => NewKey("cluster")).ToList();
        var dueAt = DateTimeOffset.UtcNow.AddSeconds(1);
        for (var i = 0; i < keys.Count; i++)
        {
            var node = i % 2 == 0 ? cluster.NodeA : cluster.NodeB;
            await node.GetRequiredService<IDeadlines>().ScheduleAsync(RecordJob.Name, keys[i], dueAt, null, Cancellation);
        }

        Assert.True(await SchedulerCluster.EventuallyAsync(
            async () => (await KeysRunAsync(RecordJob.Name, keys)).Count == keys.Count,
            Timeout));
        await Task.Delay(TimeSpan.FromSeconds(2), Cancellation);

        var runs = (await Probes.RunsAsync(RecordJob.Name)).Where(run => keys.Contains(run.Key!)).ToList();
        Assert.Equal(keys.Order(StringComparer.Ordinal), runs.Select(run => run.Key!).Order(StringComparer.Ordinal));
    }

    [Fact]
    public async Task A_failed_firing_is_retried_and_the_retry_knows_its_attempt()
    {
        var key = NewKey("retry");

        await cluster.NodeA.GetRequiredService<IDeadlines>()
            .ScheduleAsync(FailOnceJob.Name, key, DateTimeOffset.UtcNow, null, Cancellation);

        Assert.True(await SchedulerCluster.EventuallyAsync(async () => (await Probes.EffectsAsync(key)).Count == 1, Timeout));
        var attempts = (await Probes.RunsAsync(FailOnceJob.Name)).Where(run => run.Key == key).Select(run => run.Attempt);
        Assert.Equal([1, 2], attempts);
        Assert.Equal([(key, (string?)null, 2)], await Probes.EffectsAsync(key));
    }

    [Fact]
    public async Task A_cancelled_deadline_never_runs_and_cancelling_again_succeeds()
    {
        var key = NewKey("cancel");
        var deadlines = cluster.NodeA.GetRequiredService<IDeadlines>();

        await deadlines.ScheduleAsync(RecordJob.Name, key, DateTimeOffset.UtcNow.AddSeconds(2), null, Cancellation);
        await deadlines.CancelAsync(RecordJob.Name, key, Cancellation);
        await deadlines.CancelAsync(RecordJob.Name, key, Cancellation);
        await Task.Delay(TimeSpan.FromSeconds(4), Cancellation);

        Assert.DoesNotContain(await Probes.RunsAsync(RecordJob.Name), run => run.Key == key);
    }

    [Fact]
    public async Task Scheduling_a_deadline_again_replaces_its_time_and_data()
    {
        var key = NewKey("replace");
        var deadlines = cluster.NodeB.GetRequiredService<IDeadlines>();

        await deadlines.ScheduleAsync(RecordJob.Name, key, DateTimeOffset.UtcNow.AddHours(1), new Dictionary<string, string> { ["value"] = "old" }, Cancellation);
        await deadlines.ScheduleAsync(RecordJob.Name, key, DateTimeOffset.UtcNow, new Dictionary<string, string> { ["value"] = "new" }, Cancellation);

        Assert.True(await RunsReachAsync(RecordJob.Name, key, 1));
        Assert.Equal([(key, "new", 1)], await Probes.EffectsAsync(key));
    }

    [Fact]
    public async Task A_recurring_job_runs_on_its_schedule_once_per_occurrence_across_the_cluster()
    {
        Assert.True(await SchedulerCluster.EventuallyAsync(async () => (await Probes.RunsAsync(TickJob.Name)).Count >= 3, Timeout));

        var runs = await Probes.RunsAsync(TickJob.Name);
        Assert.Equal(runs.Count, runs.Select(run => run.ScheduledFor).Distinct().Count());
        Assert.All(runs, run => Assert.Null(run.Key));
    }

    [Fact]
    public async Task Recurring_jobs_no_module_registers_any_more_are_removed_at_start()
    {
        var scheduler = await cluster.NodeA.GetRequiredService<ISchedulerFactory>().GetScheduler(Cancellation);
        var gone = JobBuilder.Create<ScheduledJobRunner>().WithIdentity("Gone.Job", MemorioScheduling.RecurringGroup).StoreDurably().Build();
        await scheduler.AddJob(gone, AddJobOptions.Replacing, Cancellation);

        var definitions = cluster.NodeA.GetServices<ScheduledJobDefinition>();
        await new RecurringJobs(cluster.NodeA.GetRequiredService<ISchedulerFactory>(), definitions).StartAsync(Cancellation);

        Assert.False(await scheduler.Exists(gone.Key, Cancellation));
        Assert.True(await scheduler.Exists(new JobKey(TickJob.Name, MemorioScheduling.RecurringGroup), Cancellation));
    }

    [Fact]
    public async Task A_deadline_of_a_job_no_module_registers_is_dropped_without_retries()
    {
        var key = NewKey("unknown");
        var scheduler = await cluster.NodeA.GetRequiredService<ISchedulerFactory>().GetScheduler(Cancellation);

        await cluster.NodeA.GetRequiredService<IDeadlines>()
            .ScheduleAsync("Nobody.Registered", key, DateTimeOffset.UtcNow, null, Cancellation);

        Assert.True(await SchedulerCluster.EventuallyAsync(
            async () => await scheduler.GetTrigger(QuartzDeadlines.TriggerKeyOf("Nobody.Registered", key), Cancellation) is null,
            Timeout));
    }

    [Theory]
    [InlineData("job")]
    [InlineData("Probe.Record.Extra")]
    [InlineData("probe.record")]
    public async Task Job_names_are_Module_dot_Job(string name)
    {
        var deadlines = cluster.NodeA.GetRequiredService<IDeadlines>();

        await Assert.ThrowsAsync<ArgumentException>(() => deadlines.ScheduleAsync(name, "key", DateTimeOffset.UtcNow, null, Cancellation));
        Assert.Throws<ArgumentException>(() => new ServiceCollection().AddScheduledJob<RecordJob>(name));
    }

    private static string NewKey(string test) => $"{test}-{Guid.NewGuid():N}";

    private async Task<bool> RunsReachAsync(string job, string key, int count) =>
        await SchedulerCluster.EventuallyAsync(
            async () => (await Probes.RunsAsync(job)).Count(run => run.Key == key) >= count,
            Timeout);

    private async Task<HashSet<string>> KeysRunAsync(string job, IReadOnlyCollection<string> keys) =>
        [.. (await Probes.RunsAsync(job)).Select(run => run.Key!).Where(keys.Contains)];
}

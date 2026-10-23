using Memorio.Hosting.Configuration;
using Memorio.Hosting.Health;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Quartz;

namespace Memorio.Hosting.Scheduling;

/// <summary>
/// The worker's scheduler (ADR 0012, D-036): Quartz.NET with its store in PostgreSQL (schema
/// <see cref="Schema"/> of the role memorio_scheduler), clustered from the start, so a second worker
/// shares the jobs instead of running them twice, and a firing a dead node left behind is recovered by
/// another. Recovery and retries mean a firing can run more than once, hence idempotent jobs (SD12).
/// Modules register jobs with <c>AddScheduledJob</c> and schedule deadlines through <see cref="IDeadlines"/>;
/// they never see Quartz. Independent of Wolverine, which only carries messages (D-030).
/// </summary>
public static class MemorioScheduling
{
    public const string Schema = "scheduler";
    public const string HealthCheckName = "scheduler";

    /// <summary>Job group of the recurring jobs (one Quartz job per registered cron schedule).</summary>
    public const string RecurringGroup = "recurring";

    /// <summary>Job group of the deadline jobs (one Quartz job per job name, one trigger per key).</summary>
    public const string DeadlinesGroup = "deadlines";

    /// <param name="retryDelays">Waits between retries; <see cref="ScheduledJobRules.RetryDelays"/> unless a test shortens them.</param>
    public static IServiceCollection AddMemorioScheduling(
        this IServiceCollection services,
        IConfiguration configuration,
        IReadOnlyList<TimeSpan>? retryDelays = null)
    {
        services.AddOptions<SchedulingOptions>()
            .Bind(configuration.GetSection(SchedulingOptions.Section))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                $"{ConfigurationErrors.Variable($"{SchedulingOptions.Section}:{nameof(SchedulingOptions.ConnectionString)}")} is required.")
            .ValidateOnStart();

        // A missing value fails the validation above before Quartz connects; the placeholder only lets the
        // host be built so that validation can report it.
        var connectionString = configuration.GetSection(SchedulingOptions.Section).Get<SchedulingOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        // The role's search_path is its schema (db/bootstrap.sql), so Quartz's tables land in it; no Kerberos probe.
        connectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            GssEncryptionMode = GssEncryptionMode.Disable,
            SearchPath = Schema,
        }.ConnectionString;

        services.AddSingleton(new SchedulingMetrics());
        services.AddScoped<ScheduledJobRunner>();
        services.AddSingleton<IDeadlines, QuartzDeadlines>();
        services.AddHostedService<RecurringJobs>();

        services.AddQuartz(quartz =>
        {
            quartz.UseDefaultRetryPolicy(RetryPolicy.Explicit([.. retryDelays ?? ScheduledJobRules.RetryDelays]));
            quartz.UsePersistentStore(store =>
            {
                store.UsePostgres(connectionString);
                store.UseSystemTextJsonSerializer();
                store.UseClustering();
                store.ProvisionSchema();
                store.ConfigureStore(options =>
                {
                    // Job data is a module's own strings (keys, ids): readable rows, nothing to deserialize.
                    options.StoreJobDataAsStrings = true;

                    // A failing firing is the job's retry policy's business, never a reason to stop the trigger.
                    options.MaxConsecutiveFireFailures = 0;
                });
            });
            quartz.AddQuartzHealthChecks(health =>
            {
                health.Name = HealthCheckName;
                health.Tags.Add(HealthEndpoints.ReadyTag);
            });
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}

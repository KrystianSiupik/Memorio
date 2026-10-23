using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modularity;

/// <summary>How a module registers its scheduled jobs (ADR 0012) in <see cref="IModule.AddServices"/>.</summary>
public static class ScheduledJobs
{
    /// <summary>
    /// Registers <typeparamref name="TJob"/> under <paramref name="name"/> ("Module.Job"), run in a scope of
    /// its own on every firing. With <paramref name="cron"/> (a Quartz cron expression, in UTC) it also runs
    /// on that schedule; without, only at the deadlines the module schedules through <see cref="IDeadlines"/>.
    /// </summary>
    public static IServiceCollection AddScheduledJob<TJob>(this IServiceCollection services, string name, string? cron = null)
        where TJob : class, IScheduledJob
    {
        if (!ScheduledJobRules.IsValidName(name))
        {
            throw new ArgumentException($"A scheduled job is named \"Module.Job\", not \"{name}\".", nameof(name));
        }

        services.AddKeyedScoped<IScheduledJob, TJob>(name);
        services.AddSingleton(new ScheduledJobDefinition(name, cron));
        return services;
    }
}

/// <summary>A registered job: its name and, for a recurring one, its cron expression (UTC).</summary>
public sealed record ScheduledJobDefinition(string Name, string? Cron);

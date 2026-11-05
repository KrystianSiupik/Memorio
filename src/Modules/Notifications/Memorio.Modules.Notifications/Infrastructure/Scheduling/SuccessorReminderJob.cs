using System.Globalization;
using Memorio.Modules.Notifications.Application.Policies;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Notifications.Infrastructure.Scheduling;

/// <summary>C167: the reminder of day 7 or 21; the key is the case and the day.</summary>
internal sealed class SuccessorReminderJob(RemindSuccessor remind) : IScheduledJob
{
    public Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken) =>
        remind.HandleAsync(
            Guid.Parse(run.Key![..run.Key!.IndexOf(':', StringComparison.Ordinal)]),
            int.Parse(run.Data[NotifySuccessor.DaysKey], CultureInfo.InvariantCulture),
            cancellationToken);
}

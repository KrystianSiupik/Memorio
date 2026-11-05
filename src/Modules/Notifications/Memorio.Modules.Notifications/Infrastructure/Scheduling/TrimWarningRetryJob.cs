using System.Globalization;
using Memorio.Modules.Notifications.Application.Commands;
using Memorio.Modules.Notifications.Application.Policies;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Notifications.Infrastructure.Scheduling;

/// <summary>
/// The monthly retry of a warning that did not arrive (P26). After delivery the trim waits at least 30 more days (D-285),
/// so the message says that. Still not delivered: the same again in a month.
/// </summary>
internal sealed class TrimWarningRetryJob(WarnBeforeTrimHandler warn, RetryWhileHeld retry) : IScheduledJob
{
    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        var resourceId = Guid.Parse(run.Key!);
        var isAlbum = bool.Parse(run.Data[RetryWhileHeld.IsAlbumKey]);
        var days = int.Parse(run.Data[RetryWhileHeld.DaysKey], CultureInfo.InvariantCulture);
        var round = int.Parse(run.Data[RetryWhileHeld.RoundKey], CultureInfo.InvariantCulture);

        if (!await warn.HandleAsync(new WarnBeforeTrim(resourceId, isAlbum, days, round, "co najmniej 30 dni"), cancellationToken))
        {
            await retry.ScheduleAsync(resourceId, isAlbum, days, round, run.ScheduledFor.AddMonths(1), cancellationToken);
        }
    }
}

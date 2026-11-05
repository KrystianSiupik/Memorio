using Memorio.Modules.Album.Application.Policies;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Album.Infrastructure.Scheduling;

/// <summary>The deadline of a reservation firing is E170; decides with the time it was due (ADR 0012).</summary>
internal sealed class TakeoverReservationJob(ExpireTakeoverReservation policy) : IScheduledJob
{
    public Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken) =>
        policy.HandleAsync(Guid.Parse(run.Key!), run.ScheduledFor, cancellationToken);
}

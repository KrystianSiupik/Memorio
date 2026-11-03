using System.Globalization;
using Memorio.Modules.Archive.Application.Policies;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Archive.Infrastructure.Scheduling;

/// <summary>The deadline of a trash window firing is E21; decides with the time it was due (ADR 0012).</summary>
internal sealed class TrashWindowJob(DeleteMemoryWhenTrashWindowPassed policy) : IScheduledJob
{
    public Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken) =>
        policy.HandleAsync(
            new TrashWindowPassed(
                new MemoryId(Guid.Parse(run.Key!)),
                int.Parse(run.Data[TrashWindowDeadline.SequenceKey], CultureInfo.InvariantCulture),
                run.ScheduledFor),
            cancellationToken);
}

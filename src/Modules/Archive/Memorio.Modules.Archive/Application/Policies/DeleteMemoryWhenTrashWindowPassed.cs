using Memorio.Modules.Archive.Application.Commands.DeleteMemory;
using Memorio.Modules.Archive.Domain.Memories;

namespace Memorio.Modules.Archive.Application.Policies;

/// <summary>E21: the trash window with this sequence ended at this time.</summary>
internal sealed record TrashWindowPassed(MemoryId MemoryId, int Sequence, DateTimeOffset At);

/// <summary>
/// P3: whenever the trash window of a memory passes, delete it for good. A window already decided, or an
/// earlier one than the memory has now, is refused by the aggregate and that is the end of it.
/// </summary>
internal sealed class DeleteMemoryWhenTrashWindowPassed(DeleteMemoryHandler delete)
{
    public async Task HandleAsync(TrashWindowPassed passed, CancellationToken cancellationToken)
    {
        await delete.HandleAsync(new DeleteMemory(passed.MemoryId, passed.Sequence, passed.At), cancellationToken);
    }
}

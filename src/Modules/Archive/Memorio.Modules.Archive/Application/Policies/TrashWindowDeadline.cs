using System.Globalization;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Messaging;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Archive.Application.Policies;

/// <summary>
/// The deadline follows the window the memory has now, whatever order the events arrive in: an event about
/// an earlier window finds another sequence or state and changes nothing (D-013).
/// </summary>
internal sealed class TrashWindowDeadline(IMemories memories, IDeadlines deadlines)
    : IIntegrationEventPolicy<MemoryMovedToTrashEvent>, IIntegrationEventPolicy<MemoryRestoredEvent>
{
    public const string JobName = "Archive.TrashWindow";
    public const string SequenceKey = "sequence";

    public async Task HandleAsync(MemoryMovedToTrashEvent integrationEvent, CancellationToken cancellationToken)
    {
        var memory = await memories.FindAsync(new MemoryId(integrationEvent.MemoryId), cancellationToken);
        if (memory?.State != MemoryState.Trashed || memory.Sequence != integrationEvent.Sequence)
        {
            return;
        }

        await deadlines.ScheduleAsync(
            JobName,
            integrationEvent.MemoryId.ToString(),
            memory.WindowEndsAt!.Value,
            new Dictionary<string, string> { [SequenceKey] = memory.Sequence.ToString(CultureInfo.InvariantCulture) },
            cancellationToken);
    }

    public async Task HandleAsync(MemoryRestoredEvent integrationEvent, CancellationToken cancellationToken)
    {
        var memory = await memories.FindAsync(new MemoryId(integrationEvent.MemoryId), cancellationToken);
        if (memory?.State == MemoryState.Trashed)
        {
            return;
        }

        await deadlines.CancelAsync(JobName, integrationEvent.MemoryId.ToString(), cancellationToken);
    }
}

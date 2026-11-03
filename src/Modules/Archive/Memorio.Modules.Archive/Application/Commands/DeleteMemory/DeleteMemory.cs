using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Application.Commands.DeleteMemory;

/// <summary>C18: the trash window of a memory ended, and it goes for good. <paramref name="Sequence"/> says which window.</summary>
internal sealed record DeleteMemory(MemoryId MemoryId, int Sequence, DateTimeOffset At);

internal sealed class DeleteMemoryHandler(IMemories memories, MemoryEraser eraser)
{
    public async Task<Result> HandleAsync(DeleteMemory command, CancellationToken cancellationToken)
    {
        var memory = await memories.FindAsync(command.MemoryId, cancellationToken);
        if (memory is null)
        {
            return MemoryErrors.AlreadyDecided;
        }

        var allowed = memory.CanLeaveTrash(command.Sequence, command.At);
        if (allowed.IsFailure)
        {
            return allowed;
        }

        await eraser.EraseAsync(memory, command.At, cancellationToken);
        return await memories.SaveAsync(cancellationToken) ? Result.Success() : MemoryErrors.AlreadyDecided;
    }
}

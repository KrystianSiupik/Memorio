using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Application.Commands.DeleteMemory;

/// <summary>D-312: a memory a legal hold kept goes for good once the hold ends; while it stands, nothing changes.</summary>
internal sealed class EraseAwaitingHandler(IMemories memories, MemoryEraser eraser, TimeProvider clock)
{
    public async Task<Result> HandleAsync(MemoryId id, CancellationToken cancellationToken)
    {
        if (await memories.FindAsync(id, cancellationToken) is not { State: MemoryState.Erasing } memory)
        {
            return MemoryErrors.AlreadyDecided;
        }

        await eraser.EraseAsync(memory, clock.GetUtcNow(), cancellationToken);
        return await memories.SaveAsync(cancellationToken) ? Result.Success() : MemoryErrors.AlreadyDecided;
    }
}

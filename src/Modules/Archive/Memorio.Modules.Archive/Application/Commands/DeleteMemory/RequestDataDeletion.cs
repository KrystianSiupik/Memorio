using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Application.Commands.DeleteMemory;

/// <summary>C20: the author's formal request to delete: it skips the trash (S2:B3), from the trash too.</summary>
internal sealed record RequestDataDeletion(MemoryId MemoryId, Guid AccountId, bool Confirmed);

internal sealed class RequestDataDeletionHandler(IMemories memories, MemoryEraser eraser, TimeProvider clock)
{
    public async Task<Result> HandleAsync(RequestDataDeletion command, CancellationToken cancellationToken)
    {
        if (!command.Confirmed)
        {
            return MemoryErrors.ConfirmationRequired;
        }

        var memory = await memories.FindAsync(command.MemoryId, cancellationToken);
        if (memory is null || memory.AuthorAccountId != command.AccountId)
        {
            return MemoryErrors.NoAccess;
        }

        await eraser.EraseAsync(memory, clock.GetUtcNow(), cancellationToken);
        return await memories.SaveAsync(cancellationToken) ? Result.Success() : MemoryErrors.AlreadyDecided;
    }
}

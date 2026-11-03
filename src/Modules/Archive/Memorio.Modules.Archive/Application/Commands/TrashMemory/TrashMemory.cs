using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Application.Commands.TrashMemory;

/// <summary>C17: the author moves their content to the trash.</summary>
internal sealed record TrashMemory(MemoryId MemoryId, Guid AccountId);

internal sealed class TrashMemoryHandler(IMemories memories, TimeProvider clock)
{
    public async Task<Result> HandleAsync(TrashMemory command, CancellationToken cancellationToken)
    {
        var memory = await memories.FindAsync(command.MemoryId, cancellationToken);
        if (memory is null || memory.AuthorAccountId != command.AccountId)
        {
            return MemoryErrors.NoAccess;
        }

        memory.MoveToTrash(clock.GetUtcNow());
        return await memories.SaveAsync(cancellationToken) ? Result.Success() : MemoryErrors.AlreadyDecided;
    }
}

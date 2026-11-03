using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Application.Commands.RestoreMemory;

/// <summary>C19: the author takes their content back from the trash.</summary>
internal sealed record RestoreMemory(MemoryId MemoryId, Guid AccountId);

/// <summary>Trimmed material is refused here: it comes back through C168 (E7), which knows about the plan.</summary>
internal sealed class RestoreMemoryHandler(IMemories memories, TimeProvider clock)
{
    public async Task<Result> HandleAsync(RestoreMemory command, CancellationToken cancellationToken)
    {
        var memory = await memories.FindAsync(command.MemoryId, cancellationToken);
        if (memory is null || memory.AuthorAccountId != command.AccountId)
        {
            return MemoryErrors.NoAccess;
        }

        var restored = memory.Restore(clock.GetUtcNow(), planPaid: false);
        if (restored.IsFailure)
        {
            return restored;
        }

        return await memories.SaveAsync(cancellationToken) ? Result.Success() : MemoryErrors.AlreadyDecided;
    }
}

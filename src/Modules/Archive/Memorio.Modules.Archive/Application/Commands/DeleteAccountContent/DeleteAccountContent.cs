using Memorio.Modules.Archive.Application.Commands.DeleteMemory;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Archive.Application.Commands.DeleteAccountContent;

/// <summary>C25: everything the account recorded goes for good, without the 30 days in the trash.</summary>
internal sealed record DeleteAccountContent(Guid AccountId);

/// <summary>A second run finds nothing to do; a failure fails the delivery and the retry finds what is left.</summary>
internal sealed class DeleteAccountContentHandler(IMemories memories, MemoryEraser eraser, TimeProvider clock)
{
    public async Task<IReadOnlyList<IDomainEvent>> HandleAsync(DeleteAccountContent command, CancellationToken cancellationToken)
    {
        var deleted = new List<IDomainEvent>();
        var now = clock.GetUtcNow();
        foreach (var memory in await memories.FindByAuthorAsync(command.AccountId, cancellationToken))
        {
            await eraser.EraseAsync(memory, now, cancellationToken);
            deleted.AddRange(memory.DomainEvents);
        }

        if (!await memories.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("A memory of the deleted account changed meanwhile; the retry finds it again.");
        }

        return deleted;
    }
}

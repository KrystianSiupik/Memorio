using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Storage;

namespace Memorio.Modules.Archive.Application.Commands.DeleteAccountContent;

/// <summary>C25: everything the account recorded goes for good, without the 30 days in the trash.</summary>
internal sealed record DeleteAccountContent(Guid AccountId);

/// <summary>
/// Files first, rows last: when the storage does not answer, the exception fails the delivery and the
/// retry finds the same memories (deleting a missing file succeeds). A second run finds nothing to do.
/// </summary>
internal sealed class DeleteAccountContentHandler(IMemories memories, IObjectStorage storage, TimeProvider clock)
{
    public async Task<IReadOnlyList<IDomainEvent>> HandleAsync(DeleteAccountContent command, CancellationToken cancellationToken)
    {
        var deleted = new List<IDomainEvent>();
        var now = clock.GetUtcNow();
        foreach (var memory in await memories.FindByAuthorAsync(command.AccountId, cancellationToken))
        {
            foreach (var photo in memory.Photos)
            {
                await storage.DeleteAsync(new StorageKey(photo.StorageKey), cancellationToken);
            }

            memory.DeleteDefinitively(now);
            deleted.AddRange(memory.DomainEvents);
            memories.Remove(memory);
        }

        await memories.SaveAsync(cancellationToken);
        return deleted;
    }
}

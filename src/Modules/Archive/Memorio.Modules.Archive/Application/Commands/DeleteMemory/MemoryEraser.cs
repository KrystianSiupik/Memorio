using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Moderation.Contracts;
using Memorio.SharedKernel.Storage;

namespace Memorio.Modules.Archive.Application.Commands.DeleteMemory;

/// <summary>
/// The one way a memory goes for good, whoever asks. Space and files first, the row last: when something does
/// not answer, the exception fails the attempt and the retry finds the memory again. Giving space back and
/// deleting a missing file are both safe to repeat. The caller saves, so the row and its events go together.
/// While an order of an authority holds the author's data, nothing goes: the memory waits unseen (C114, D-312).
/// </summary>
internal sealed class MemoryEraser(IMemories memories, ISpacePool pool, IObjectStorage storage, ILegalHolds holds)
{
    public async Task EraseAsync(Memory memory, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (await holds.IsHeldAsync(memory.AuthorAccountId, cancellationToken))
        {
            memory.AwaitErasure(now);
            return;
        }

        foreach (var photo in memory.Photos)
        {
            await pool.ReleaseAsync(photo.Id, cancellationToken);
            await storage.DeleteAsync(new StorageKey(photo.StorageKey), cancellationToken);
        }

        memory.DeleteDefinitively(now);
        memories.Remove(memory);
    }
}

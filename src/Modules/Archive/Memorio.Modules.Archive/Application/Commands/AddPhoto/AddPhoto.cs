using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Limits.Contracts;
using Memorio.SharedKernel.Results;
using Memorio.SharedKernel.Storage;

namespace Memorio.Modules.Archive.Application.Commands.AddPhoto;

/// <summary>C13: an uploaded file becomes a photo of a memory. The photo id comes from the client: sending it again changes nothing.</summary>
/// <param name="AlbumId">The album whose pool pays for the photo; otherwise the author's.</param>
internal sealed record AddPhoto(MemoryId MemoryId, Guid AccountId, Guid PhotoId, string? UploadKey, Guid? AlbumId = null);

/// <summary>
/// The space is reserved when the photo is stored, with the photo id as its key (D-015). When the pool is
/// full or storing fails, the file goes and the space comes back, so nothing stays half done.
/// </summary>
internal sealed class AddPhotoHandler(IMemories memories, IObjectStorage storage, ISpacePool pool, IAlbumPool albumPool, TimeProvider clock)
{
    public async Task<Result> HandleAsync(AddPhoto command, CancellationToken cancellationToken)
    {
        var memory = await memories.FindAsync(command.MemoryId, cancellationToken);
        if (memory is null || memory.AuthorAccountId != command.AccountId)
        {
            return MemoryErrors.NoAccess;
        }

        if (memory.State != MemoryState.Active)
        {
            return MemoryErrors.AlreadyDecided;
        }

        if (memory.HasPhoto(command.PhotoId))
        {
            return Result.Success();
        }

        if (!TryUploadKey(command.UploadKey, out var upload))
        {
            return MemoryErrors.InvalidUpload;
        }

        var destination = new StorageKey($"photos/{command.PhotoId}/original");
        var stored = await storage.PromoteUploadAsync(upload, destination, cancellationToken);
        if (stored is null)
        {
            return MemoryErrors.UploadMissing;
        }

        var fits = command.AlbumId is { } albumId
            ? await albumPool.TryReserveAsync(albumId, command.PhotoId, stored.Size, cancellationToken)
            : await pool.TryReserveAsync(command.AccountId, command.PhotoId, stored.Size, cancellationToken);
        if (!fits)
        {
            await storage.DeleteAsync(destination, cancellationToken);
            return MemoryErrors.SpaceLimitExceeded;
        }

        memory.AddPhoto(command.PhotoId, destination.Value, stored.Size, clock.GetUtcNow(), command.AlbumId);
        if (await memories.SaveAsync(cancellationToken))
        {
            return Result.Success();
        }

        await pool.ReleaseAsync(command.PhotoId, cancellationToken);
        await storage.DeleteAsync(destination, cancellationToken);
        return MemoryErrors.AlreadyDecided;
    }

    private static bool TryUploadKey(string? value, out StorageKey key)
    {
        key = default;
        try
        {
            key = new StorageKey(value!);
            return key.IsUpload;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}

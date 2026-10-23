namespace Memorio.SharedKernel.Storage;

/// <summary>
/// Port to the private object storage in the EU (ADR 0005). The bucket has no public access: media are
/// read and written only through signed addresses valid for <see cref="ObjectStorageRules.SignatureLifetime"/>,
/// handed out after the Access module allowed it (ADR 0011).
/// </summary>
public interface IObjectStorage
{
    /// <summary>Signed read addresses for a batch of objects (a gallery at once), all with the same expiry.</summary>
    Task<IReadOnlyDictionary<StorageKey, SignedUrl>> SignReadsAsync(IReadOnlyCollection<StorageKey> keys, CancellationToken cancellationToken);

    /// <summary>A new key in the upload zone and a signed PUT address for it; the upload must send <paramref name="contentType"/>.</summary>
    Task<UploadSlot> CreateUploadAsync(string contentType, CancellationToken cancellationToken);

    /// <summary>
    /// Moves an uploaded object from the upload zone to <paramref name="destination"/> and returns what
    /// was stored; null when the upload does not exist (never uploaded or already expired).
    /// </summary>
    Task<StoredObject?> PromoteUploadAsync(StorageKey upload, StorageKey destination, CancellationToken cancellationToken);

    /// <summary>Deletes the object; deleting a missing object succeeds (idempotent).</summary>
    Task DeleteAsync(StorageKey key, CancellationToken cancellationToken);
}

public sealed record SignedUrl(Uri Url, DateTimeOffset ExpiresAt);

public sealed record UploadSlot(StorageKey Key, SignedUrl Upload, string ContentType);

public sealed record StoredObject(StorageKey Key, long Size, string ContentType);

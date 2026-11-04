namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>A photo of a memory; the file itself is in the object storage under <see cref="StorageKey"/>.</summary>
internal sealed class Photo
{
    private Photo()
    {
    }

    /// <summary>From the client; also the key of its reservation in the pool (D-015).</summary>
    public Guid Id { get; private set; }

    public string StorageKey { get; private set; } = "";

    public long SizeBytes { get; private set; }

    /// <summary>The album whose pool pays for this photo; null means the author's.</summary>
    public Guid? AlbumId { get; private set; }

    public static Photo Create(Guid id, string storageKey, long sizeBytes, Guid? albumId = null) =>
        new() { Id = id, StorageKey = storageKey, SizeBytes = sizeBytes, AlbumId = albumId };

    public void LeaveAlbumPool() => AlbumId = null;
}

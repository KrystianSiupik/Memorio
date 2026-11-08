namespace Memorio.Modules.Provenance.Domain.Artifacts;

/// <summary>
/// A fact that some content may no longer be used, with its time (D-023). Only ever added: a registration checks the facts
/// since it read its materials, so an artifact made just before a withdrawal does not survive it (AG9).
/// </summary>
internal sealed class UnavailableSource
{
    private UnavailableSource(Guid id, UnavailableKind kind, Guid? memoryId, Guid? albumId, Guid? accountId, InvalidationReason reason, DateTimeOffset at)
    {
        Id = id;
        Kind = kind;
        MemoryId = memoryId;
        AlbumId = albumId;
        AccountId = accountId;
        Reason = reason;
        At = at;
    }

    /// <summary>The id of the event that brought it: a second delivery adds nothing.</summary>
    public Guid Id { get; private set; }

    public UnavailableKind Kind { get; private set; }

    public Guid? MemoryId { get; private set; }

    public Guid? AlbumId { get; private set; }

    public Guid? AccountId { get; private set; }

    public InvalidationReason Reason { get; private set; }

    public DateTimeOffset At { get; private set; }

    /// <summary>P4: the memory is gone for good, wherever it was used.</summary>
    public static UnavailableSource Memory(Guid eventId, Guid memoryId, InvalidationReason reason, DateTimeOffset at) =>
        new(eventId, UnavailableKind.Memory, memoryId, null, null, reason, at);

    /// <summary>P34: the memory left this album only (D-290).</summary>
    public static UnavailableSource MemoryInAlbum(Guid eventId, Guid memoryId, Guid albumId, InvalidationReason reason, DateTimeOffset at) =>
        new(eventId, UnavailableKind.MemoryInAlbum, memoryId, albumId, null, reason, at);

    /// <summary>P9, P12: what this author brought to this album may not be used any more.</summary>
    public static UnavailableSource AuthorInAlbum(Guid eventId, Guid albumId, Guid authorAccountId, InvalidationReason reason, DateTimeOffset at) =>
        new(eventId, UnavailableKind.AuthorInAlbum, null, albumId, authorAccountId, reason, at);

    /// <summary>P10: this person lost the album, with everything made for them from it.</summary>
    public static UnavailableSource ReaderOfAlbum(Guid eventId, Guid albumId, Guid readerAccountId, InvalidationReason reason, DateTimeOffset at) =>
        new(eventId, UnavailableKind.ReaderOfAlbum, null, albumId, readerAccountId, reason, at);

    /// <summary>Whether the artifact was made from what this fact takes away.</summary>
    public bool Hits(Artifact artifact) => Kind switch
    {
        UnavailableKind.Memory => artifact.Sources.Any(source => source.MemoryId == MemoryId),
        UnavailableKind.MemoryInAlbum => artifact.Sources.Any(source => source.MemoryId == MemoryId && source.AlbumId == AlbumId),
        UnavailableKind.AuthorInAlbum => artifact.Sources.Any(source => source.AlbumId == AlbumId && source.AuthorAccountId == AccountId),
        UnavailableKind.ReaderOfAlbum => artifact.AlbumId == AlbumId && artifact.OwnerAccountId == AccountId,
        _ => false,
    };
}

internal enum UnavailableKind
{
    Memory,
    MemoryInAlbum,
    AuthorInAlbum,
    ReaderOfAlbum,
}

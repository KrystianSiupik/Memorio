namespace Memorio.Modules.Archive.Contracts;

/// <summary>Ports for Album: memories stay here, an album only points at them (D-248).</summary>
public interface IAlbumContent
{
    /// <summary>Null when there is no such active memory.</summary>
    Task<AlbumMemoryFacts?> DescribeAsync(Guid memoryId, CancellationToken cancellationToken);

    /// <summary>Only active memories: what is in the trash is not shown in an album.</summary>
    Task<IReadOnlyDictionary<Guid, AlbumMemorySummary>> SummarizeAsync(IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken);

    /// <summary>People in the album can see the memory from now on; safe to call again.</summary>
    Task LinkAsync(Guid memoryId, Guid albumId, CancellationToken cancellationToken);

    /// <summary>The memory no longer is in the album; photos its pool paid for go back to the author's. Safe to call again.</summary>
    Task UnlinkAsync(Guid memoryId, Guid albumId, CancellationToken cancellationToken);

    /// <summary>R7 for C71: the person's own active memories in a period, oldest first; only those may go into an album (D-248).</summary>
    Task<IReadOnlyList<Guid>> OwnInPeriodAsync(Guid accountId, DateOnly from, DateOnly to, CancellationToken cancellationToken);

    /// <summary>What goes when this album comes down to the limit: the owner's choice stays first, then the oldest (D-284).</summary>
    Task<IReadOnlyList<Guid>> ChooseToTrimAsync(Guid albumId, IReadOnlyCollection<Guid> keptMemoryIds, long limitBytes, CancellationToken cancellationToken);

    /// <summary>What the trim of this album could take, oldest first (S19).</summary>
    Task<IReadOnlyList<AlbumTrimMaterial>> TrimMaterialAsync(Guid albumId, CancellationToken cancellationToken);

    /// <summary>C56: stores the photo with the album's pool paying for it (MEM-E3-ARCHIWUM-01).</summary>
    Task<AlbumPhotoOutcome> AddPhotoAsync(AlbumPhoto photo, CancellationToken cancellationToken);
}

public sealed record AlbumMemoryFacts(Guid AuthorAccountId, long SizeBytes);

public sealed record AlbumMemorySummary(string PlaceName, string DateFrom, int Photos);

public sealed record AlbumTrimMaterial(Guid MemoryId, string PlaceName, string DateFrom, long SizeBytes);

public sealed record AlbumPhoto(Guid AlbumId, Guid MemoryId, Guid AccountId, Guid PhotoId, string? UploadKey);

public enum AlbumPhotoOutcome
{
    Added,
    AlreadyThere,
    NoAccess,
    PoolFull,
    UploadMissing,
}

namespace Memorio.Modules.Album.Domain.Albums;

/// <summary>A memory its author brought to an album. The memory itself stays in Archive.</summary>
internal sealed class AlbumMemory
{
    private readonly List<MemoryTag> _tags = [];

    private AlbumMemory()
    {
    }

    public AlbumId AlbumId { get; private set; }

    public Guid MemoryId { get; private set; }

    public Guid AuthorAccountId { get; private set; }

    public DateTimeOffset AddedAt { get; private set; }

    /// <summary>Set by a curator; null keeps the order of adding.</summary>
    public int? Position { get; private set; }

    public IReadOnlyCollection<MemoryTag> Tags => _tags.AsReadOnly();

    public static AlbumMemory Create(AlbumId albumId, Guid memoryId, Guid authorAccountId, DateTimeOffset now) =>
        new() { AlbumId = albumId, MemoryId = memoryId, AuthorAccountId = authorAccountId, AddedAt = now };

    public void MoveTo(int position) => Position = position;

    public bool Tag(Guid accountId, Guid byAccountId, DateTimeOffset now)
    {
        if (_tags.Exists(tag => tag.AccountId == accountId))
        {
            return false;
        }

        _tags.Add(new MemoryTag(accountId, byAccountId, now));
        return true;
    }

    public bool Untag(Guid accountId) => _tags.RemoveAll(tag => tag.AccountId == accountId) > 0;
}

/// <summary>D-079: only an account, and only of someone in the same album.</summary>
internal sealed record MemoryTag(Guid AccountId, Guid TaggedByAccountId, DateTimeOffset TaggedAt);

/// <summary>An author lets a curator put their contributions in order (C49). One curator per author and album.</summary>
internal sealed class Entrustment
{
    private Entrustment()
    {
    }

    public AlbumId AlbumId { get; private set; }

    public Guid AuthorAccountId { get; private set; }

    public Guid CuratorAccountId { get; private set; }

    public DateTimeOffset EntrustedAt { get; private set; }

    public static Entrustment Create(AlbumId albumId, Guid authorAccountId, Guid curatorAccountId, DateTimeOffset now) =>
        new() { AlbumId = albumId, AuthorAccountId = authorAccountId, CuratorAccountId = curatorAccountId, EntrustedAt = now };

    public void HandOver(Guid curatorAccountId, DateTimeOffset now)
    {
        CuratorAccountId = curatorAccountId;
        EntrustedAt = now;
    }
}

/// <summary>Contributions that stayed after their author left or was removed; revocable at any time (C55, D-249).</summary>
internal sealed class KeptContribution
{
    private KeptContribution()
    {
    }

    public AlbumId AlbumId { get; private set; }

    public Guid AuthorAccountId { get; private set; }

    public DateTimeOffset KeptAt { get; private set; }

    /// <summary>False when the owner removed the person and they have not chosen yet.</summary>
    public bool ChosenByAuthor { get; private set; }

    public static KeptContribution Create(AlbumId albumId, Guid authorAccountId, bool chosenByAuthor, DateTimeOffset now) =>
        new() { AlbumId = albumId, AuthorAccountId = authorAccountId, ChosenByAuthor = chosenByAuthor, KeptAt = now };
}

/// <summary>The last event about a deleted account applied here; an older one changes nothing (D-013).</summary>
internal sealed class AppliedAccountEvent(Guid accountId, long sequence)
{
    public Guid AccountId { get; private set; } = accountId;

    public long Sequence { get; private set; } = sequence;

    public void MoveTo(long sequence) => Sequence = sequence;
}

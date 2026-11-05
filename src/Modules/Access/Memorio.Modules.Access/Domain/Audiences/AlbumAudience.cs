using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Access.Domain.Audiences;

/// <summary>
/// AG3: who may read an album without being in it, and who may not come in at all. Every change grows
/// <see cref="Sequence"/>, the concurrency token, so a block or a revoked link and an entry never both win (D-009, D-269).
/// </summary>
internal sealed class AlbumAudience : AggregateRoot<Guid>
{
    private readonly List<AlbumViewer> _viewers = [];
    private readonly List<AlbumBlock> _blocks = [];

    private AlbumAudience(Guid id)
        : base(id)
    {
    }

    /// <summary>What the link carries; null when there is no working link.</summary>
    public string? LinkToken { get; private set; }

    /// <summary>Grows with every change; events carry it (D-013).</summary>
    public int Sequence { get; private set; } = 1;

    public IReadOnlyCollection<AlbumViewer> Viewers => _viewers.AsReadOnly();

    public IReadOnlyCollection<AlbumBlock> Blocks => _blocks.AsReadOnly();

    public static AlbumAudience For(Guid albumId) => new(albumId);

    public bool IsBlocked(Guid accountId) => _blocks.Exists(block => block.AccountId == accountId);

    public bool IsViewer(Guid accountId) => _viewers.Exists(viewer => viewer.AccountId == accountId);

    /// <summary>C60. A working link stays the same, so what was already sent keeps working.</summary>
    public string GenerateLink(string newToken, DateTimeOffset now)
    {
        if (LinkToken is { } existing)
        {
            return existing;
        }

        LinkToken = newToken;
        Sequence++;
        Raise(new LinkGenerated(Guid.CreateVersion7(), now, Id));
        return newToken;
    }

    /// <summary>C62: nobody new comes in; those let in keep their access.</summary>
    public Result RevokeLink(DateTimeOffset now)
    {
        if (LinkToken is null)
        {
            return AccessErrors.AlreadyDecided;
        }

        LinkToken = null;
        Sequence++;
        Raise(new LinkRevoked(Guid.CreateVersion7(), now, Id));
        return Result.Success();
    }

    /// <summary>C61. Whose access was taken away may come back through a working link (D-009).</summary>
    public Result Enter(string token, Guid accountId, DateTimeOffset now)
    {
        if (LinkToken != token)
        {
            return AccessErrors.LinkRevoked;
        }

        if (IsBlocked(accountId))
        {
            return AccessErrors.Blocked;
        }

        if (!IsViewer(accountId))
        {
            _viewers.Add(new AlbumViewer(accountId, now));
            Sequence++;
            Raise(new ViewerAdded(Guid.CreateVersion7(), now, Id, accountId));
        }

        return Result.Success();
    }

    /// <summary>C63.</summary>
    public Result RevokeAccess(Guid accountId, DateTimeOffset now)
    {
        if (_viewers.RemoveAll(viewer => viewer.AccountId == accountId) == 0)
        {
            return AccessErrors.AlreadyDecided;
        }

        Sequence++;
        Raise(new AccessRevoked(Guid.CreateVersion7(), now, Id, accountId, Sequence));
        return Result.Success();
    }

    /// <summary>C64: gone at once, and no way back through a link or an invitation. Asking again changes nothing.</summary>
    public void Block(Guid accountId, DateTimeOffset now)
    {
        if (IsBlocked(accountId))
        {
            return;
        }

        _viewers.RemoveAll(viewer => viewer.AccountId == accountId);
        _blocks.Add(new AlbumBlock(accountId, now));
        Sequence++;
        Raise(new UserBlocked(Guid.CreateVersion7(), now, Id, accountId, Sequence));
    }

    /// <summary>
    /// The owner takes a block back (D-276). It gives nothing back by itself: someone let in through a link comes
    /// in again through a working link, and someone who was in the album needs a new invitation.
    /// </summary>
    public Result Unblock(Guid accountId, DateTimeOffset now)
    {
        if (_blocks.RemoveAll(block => block.AccountId == accountId) == 0)
        {
            return AccessErrors.AlreadyDecided;
        }

        Sequence++;
        Raise(new UserUnblocked(Guid.CreateVersion7(), now, Id, accountId));
        return Result.Success();
    }

    /// <summary>C65: for a rule, not a person asking. A block wins (D-022).</summary>
    public Result GrantRead(Guid accountId, DateTimeOffset now)
    {
        if (IsBlocked(accountId))
        {
            return AccessErrors.Blocked;
        }

        if (!IsViewer(accountId))
        {
            _viewers.Add(new AlbumViewer(accountId, now));
            Sequence++;
            Raise(new ReadGranted(Guid.CreateVersion7(), now, Id, accountId));
        }

        return Result.Success();
    }
}

internal sealed record AlbumViewer(Guid AccountId, DateTimeOffset EnteredAt);

internal sealed record AlbumBlock(Guid AccountId, DateTimeOffset BlockedAt);

/// <summary>E62.</summary>
internal sealed record LinkGenerated(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId) : IDomainEvent;

/// <summary>E63.</summary>
internal sealed record ViewerAdded(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId) : IDomainEvent;

/// <summary>E65.</summary>
internal sealed record LinkRevoked(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId) : IDomainEvent;

/// <summary>E66.</summary>
internal sealed record AccessRevoked(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId, int Sequence) : IDomainEvent;

/// <summary>E67.</summary>
internal sealed record UserBlocked(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId, int Sequence) : IDomainEvent;

internal sealed record UserUnblocked(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId) : IDomainEvent;

/// <summary>E160.</summary>
internal sealed record ReadGranted(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId) : IDomainEvent;

internal static class AccessErrors
{
    /// <summary>The same answer for a link revoked and one that never existed (E64).</summary>
    public static readonly Error LinkRevoked = Error.Conflict("Access.LinkRevoked", "This link no longer works.");

    public static readonly Error Blocked = Error.Forbidden("Access.Blocked", "No access to this album.");

    public static readonly Error CannotBlockOwner = Error.Validation("Access.CannotBlockOwner", "The owner of an album cannot be blocked.");

    public static readonly Error CannotBlockYourself = Error.Validation("Access.CannotBlockYourself", "You cannot block yourself.");

    public static readonly Error OnlyOwnerBlocksParticipant = Error.Forbidden("Access.OnlyOwnerBlocksParticipant", "Only the owner can block someone in the album.");

    public static readonly Error AlreadyDecided = Error.Conflict("Access.AlreadyDecided", "This has already been decided.");

    /// <summary>The same answer for an album that does not exist and one the person has no say in.</summary>
    public static readonly Error NoAccess = Error.Forbidden("Access.NoAccess", "No access to this album.");
}

internal interface IAudiences
{
    Task<AlbumAudience?> FindAsync(Guid albumId, CancellationToken cancellationToken);

    Task<AlbumAudience?> FindByTokenAsync(string token, CancellationToken cancellationToken);

    void Add(AlbumAudience audience);

    /// <summary>A deleted account leaves no trace in any audience.</summary>
    Task ForgetAsync(Guid accountId, CancellationToken cancellationToken);

    /// <summary>Asking again changes nothing.</summary>
    Task GrantLegacyAsync(LegacyGrant grant, CancellationToken cancellationToken);

    /// <summary>Stores the changes with their events, in one transaction. False when another decision got there first.</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}

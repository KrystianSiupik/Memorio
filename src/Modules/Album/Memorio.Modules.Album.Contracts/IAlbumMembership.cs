namespace Memorio.Modules.Album.Contracts;

/// <summary>Read port for Access: who is in an album is decided here, and is true at once (D-250).</summary>
public interface IAlbumMembership
{
    Task<bool> IsMemberAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken);

    /// <summary>Null when there is no such album, or it has no owner.</summary>
    Task<Guid?> OwnerOfAsync(Guid albumId, CancellationToken cancellationToken);

    /// <summary>The albums the person owns or takes part in.</summary>
    Task<IReadOnlyCollection<Guid>> AlbumsOfAsync(Guid accountId, CancellationToken cancellationToken);
}

/// <summary>Read port for Access: what to call albums on "Shared with me".</summary>
public interface IAlbumNames
{
    Task<IReadOnlyDictionary<Guid, string>> NamesOfAsync(IReadOnlyCollection<Guid> albumIds, CancellationToken cancellationToken);
}

/// <summary>Read port for Notifications: where an event's message goes.</summary>
public interface IAlbumMessaging
{
    /// <summary>Null when the invitation no longer waits for an answer: nothing to write about.</summary>
    Task<PendingInvitation?> FindPendingInvitationAsync(Guid invitationId, CancellationToken cancellationToken);

    /// <summary>The owner and the participants: who hears about what happens to the album (S2:B1).</summary>
    Task<IReadOnlyCollection<Guid>> MembersOfAsync(Guid albumId, CancellationToken cancellationToken);
}

/// <param name="Email">The address the invitation goes to.</param>
/// <param name="InvitedByAccountId">Who sent it; the message names them (D-268).</param>
public sealed record PendingInvitation(Guid AlbumId, string Email, Guid InvitedByAccountId);

/// <summary>Write port for Succession (C41): the fact of who is named stays with the album.</summary>
public interface IAlbumSuccession
{
    Task<SuccessorOutcome> NameAsync(Guid albumId, Guid ownerAccountId, Guid successorAccountId, CancellationToken cancellationToken);

    /// <summary>The albums the person owns, each with its successor at this moment (D-020).</summary>
    Task<IReadOnlyList<OwnedAlbum>> OwnedByAsync(Guid accountId, CancellationToken cancellationToken);

    /// <summary>C122: the album goes to the successor. Safe to call again; if it is not this owner's any more, nothing happens.</summary>
    Task TransferAsync(Guid albumId, Guid fromAccountId, Guid toAccountId, CancellationToken cancellationToken);

    /// <summary>C153: the album is without an owner. Safe to call again; if it is not this owner's any more, nothing happens.</summary>
    Task ReleaseOwnerAsync(Guid albumId, Guid ownerAccountId, CancellationToken cancellationToken);

    /// <summary>How many memories of this author are in the album (R17).</summary>
    Task<int> MemoriesOfAsync(Guid albumId, Guid authorAccountId, CancellationToken cancellationToken);
}

public sealed record OwnedAlbum(Guid AlbumId, Guid? SuccessorAccountId);

public enum SuccessorOutcome
{
    Named,
    NoAccess,
    NotParticipant,
    AlreadyDecided,
}

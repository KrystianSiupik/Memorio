namespace Memorio.Modules.Album.Contracts;

/// <summary>Read port for Access: who is in an album is decided here, and is true at once (D-250).</summary>
public interface IAlbumMembership
{
    Task<bool> IsMemberAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken);

    /// <summary>Null when there is no such album.</summary>
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
}

/// <param name="Email">The address the invitation goes to.</param>
/// <param name="InvitedByAccountId">Who sent it; the message names them (D-268).</param>
public sealed record PendingInvitation(Guid AlbumId, string Email, Guid InvitedByAccountId);

/// <summary>Write port for Succession (C41): the fact of who is named stays with the album.</summary>
public interface IAlbumSuccession
{
    Task<SuccessorOutcome> NameAsync(Guid albumId, Guid ownerAccountId, Guid successorAccountId, CancellationToken cancellationToken);
}

public enum SuccessorOutcome
{
    Named,
    NoAccess,
    NotParticipant,
    AlreadyDecided,
}

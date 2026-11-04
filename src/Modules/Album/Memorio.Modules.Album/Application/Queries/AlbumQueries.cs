namespace Memorio.Modules.Album.Application.Queries;

internal sealed record AlbumListItem(Guid Id, string Name, Guid OwnerAccountId, int Members, int Memories);

internal sealed record AlbumMemberRow(Guid AccountId, bool IsOwner);

/// <summary>A class with setters: Dapper cannot match the array column to a constructor.</summary>
internal sealed class AlbumMemoryRow
{
    public Guid MemoryId { get; init; }

    public Guid AuthorAccountId { get; init; }

    public int? Position { get; init; }

    public Guid[] TaggedAccountIds { get; init; } = [];
}

/// <summary>R13: what a person in the album sees of it; the facts of memories come from Archive.</summary>
internal sealed record AlbumView(
    Guid Id,
    string Name,
    Guid OwnerAccountId,
    Guid? SuccessorAccountId,
    IReadOnlyList<AlbumMemberRow> Members,
    IReadOnlyList<AlbumMemoryRow> Memories);

/// <summary>R5.</summary>
internal sealed record AlbumSeats(int Taken, int Invited, int Free, IReadOnlyList<InvitedRow> Invitations);

internal sealed record InvitedRow(Guid Id, string Email);

/// <summary>R19: who invites and to what, without any content.</summary>
internal sealed record InvitationView(Guid Id, Guid AlbumId, string AlbumName, Guid InvitedByAccountId);

internal sealed record KeptRow(Guid AlbumId, string AlbumName, bool ChosenByAuthor, int Memories);

internal sealed record MyEntrustment(Guid AlbumId, Guid CuratorAccountId);

internal sealed record EntrustedRow(Guid AlbumId, string AlbumName, Guid MemoryId, Guid AuthorAccountId, int? Position);

internal interface IAlbumQueries
{
    Task<IReadOnlyList<AlbumListItem>> MyAlbumsAsync(Guid accountId, CancellationToken cancellationToken);

    /// <summary>The caller has asked Access: this only reads.</summary>
    Task<AlbumView?> ViewAsync(Guid albumId, CancellationToken cancellationToken);

    /// <summary>Null when the person is not the owner.</summary>
    Task<(int Participants, IReadOnlyList<InvitedRow> Invited)?> SeatsAsync(Guid albumId, Guid ownerAccountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<InvitationView>> PendingInvitationsAsync(string email, CancellationToken cancellationToken);

    Task<IReadOnlyList<KeptRow>> KeptContributionsAsync(Guid accountId, CancellationToken cancellationToken);

    Task<MyEntrustment?> MyEntrustmentAsync(Guid albumId, Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<EntrustedRow>> EntrustedToAsync(Guid curatorAccountId, CancellationToken cancellationToken);
}

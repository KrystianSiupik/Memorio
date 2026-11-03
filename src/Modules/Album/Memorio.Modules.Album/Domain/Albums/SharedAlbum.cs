using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Album.Domain.Albums;

/// <summary>
/// A collection people build together. Only what deleting an account needs so far: the owner, the successor
/// and the other participants. The rest comes with creating an album (E3) as an expanding migration.
/// </summary>
internal sealed class SharedAlbum : AggregateRoot<AlbumId>
{
    private readonly List<AlbumParticipant> _participants = [];

    private SharedAlbum(AlbumId id, Guid ownerAccountId, Guid? successorAccountId)
        : base(id)
    {
        OwnerAccountId = ownerAccountId;
        SuccessorAccountId = successorAccountId;
    }

    public Guid OwnerAccountId { get; private set; }

    public Guid? SuccessorAccountId { get; private set; }

    public IReadOnlyCollection<AlbumParticipant> Participants => _participants.AsReadOnly();

    public static SharedAlbum Create(Guid ownerAccountId, Guid? successorAccountId = null) =>
        new(new AlbumId(Guid.CreateVersion7()), ownerAccountId, successorAccountId);

    public void AddParticipant(Guid accountId) => _participants.Add(new AlbumParticipant(accountId));
}

internal sealed record AlbumParticipant(Guid AccountId);

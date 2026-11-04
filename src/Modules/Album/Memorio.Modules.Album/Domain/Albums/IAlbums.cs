namespace Memorio.Modules.Album.Domain.Albums;

internal interface IAlbums
{
    Task<SharedAlbum?> FindAsync(AlbumId id, CancellationToken cancellationToken);

    Task<SharedAlbum?> FindByInvitationAsync(Guid invitationId, CancellationToken cancellationToken);

    void Add(SharedAlbum album);

    Task<AlbumMemory?> FindMemoryAsync(AlbumId albumId, Guid memoryId, CancellationToken cancellationToken);

    Task<IReadOnlyList<AlbumMemory>> FindMemoriesOfAuthorAsync(AlbumId albumId, Guid authorAccountId, CancellationToken cancellationToken);

    /// <summary>In every album: what goes when the memory goes.</summary>
    Task<IReadOnlyList<AlbumMemory>> FindPlacesOfMemoryAsync(Guid memoryId, CancellationToken cancellationToken);

    void Add(AlbumMemory memory);

    void Remove(AlbumMemory memory);

    Task<Entrustment?> FindEntrustmentAsync(AlbumId albumId, Guid authorAccountId, CancellationToken cancellationToken);

    void Add(Entrustment entrustment);

    void Remove(Entrustment entrustment);

    Task<KeptContribution?> FindKeptAsync(AlbumId albumId, Guid authorAccountId, CancellationToken cancellationToken);

    void Add(KeptContribution kept);

    void Remove(KeptContribution kept);

    /// <summary>Albums of other owners the person is in or brought something to.</summary>
    Task<IReadOnlyList<AlbumId>> FindOtherPeoplesAlbumsAsync(Guid accountId, CancellationToken cancellationToken);

    Task<AppliedAccountEvent?> FindAppliedAsync(Guid accountId, CancellationToken cancellationToken);

    void Add(AppliedAccountEvent applied);

    /// <summary>Stores the changes with the events of the changed albums, in one transaction. False when another decision got there first.</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}

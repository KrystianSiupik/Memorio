namespace Memorio.Modules.Album.Domain.Albums;

internal interface IAlbums
{
    Task<SharedAlbum?> FindAsync(AlbumId id, CancellationToken cancellationToken);

    Task<SharedAlbum?> FindByInvitationAsync(Guid invitationId, CancellationToken cancellationToken);

    void Add(SharedAlbum album);

    /// <summary>False when the account has no invitation left on this day. Two parallel calls never pass the limit together.</summary>
    Task<bool> TryConsumeInvitationAsync(Guid accountId, DateOnly day, CancellationToken cancellationToken);

    Task ForgetInvitationQuotaAsync(Guid accountId, CancellationToken cancellationToken);

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

    Task<IReadOnlyList<SharedAlbum>> FindOwnedByAsync(Guid accountId, CancellationToken cancellationToken);

    /// <summary>Albums of other owners the person is in or brought something to.</summary>
    Task<IReadOnlyList<AlbumId>> FindOtherPeoplesAlbumsAsync(Guid accountId, CancellationToken cancellationToken);

    Task<AppliedAccountEvent?> FindAppliedAsync(Guid accountId, CancellationToken cancellationToken);

    void Add(AppliedAccountEvent applied);

    Task<AppliedPlanEvent?> FindAppliedPlanEventAsync(AlbumId albumId, CancellationToken cancellationToken);

    void RecordApplied(AppliedPlanEvent? applied, AlbumId albumId, int sequence);

    Task<IReadOnlyList<TrimmedAlbumMemory>> FindTrimmedAsync(AlbumId albumId, CancellationToken cancellationToken);

    void Add(TrimmedAlbumMemory trimmed);

    void Remove(TrimmedAlbumMemory trimmed);

    /// <summary>A memory or account that is gone leaves no trace here.</summary>
    Task ForgetTrimmedAsync(Guid? memoryId, Guid? authorAccountId, CancellationToken cancellationToken);

    /// <summary>Stores the changes with the events of the changed albums, in one transaction. False when another decision got there first.</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}

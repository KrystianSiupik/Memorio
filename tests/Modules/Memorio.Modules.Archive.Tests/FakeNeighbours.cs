using System.Collections.Concurrent;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.AiNarration.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Places.Contracts;

namespace Memorio.Modules.Archive.Tests;

/// <summary>The other modules Archive asks: every place exists but "missing", everybody accepted the terms unless told otherwise.</summary>
public sealed class FakeNeighbours : IPlaceReader, ITermsReader, ISpacePool, IAlbumPool, IAiDraftReader, IAccessReader
{
    private readonly ConcurrentDictionary<(Guid Draft, Guid Account), AiDraftInfo> _drafts = new();

    public void AddDraft(Guid draftId, Guid accountId) => _drafts[(draftId, accountId)] = new AiDraftInfo("Szkic.", "test-model", "draft@1");

    public Task<AiDraftInfo?> FindAsync(Guid draftId, Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult(_drafts.GetValueOrDefault((draftId, accountId)));

    public const string MissingPlace = "osm:N:404";

    private readonly ConcurrentDictionary<(Guid Person, Guid Album), bool> _visible = new();

    public void Sees(Guid personId, Guid albumId) => _visible[(personId, albumId)] = true;

    public void NoLongerSees(Guid personId, Guid albumId)
    {
        _visible.TryRemove((personId, albumId), out _);
        _takingPart.TryRemove((personId, albumId), out _);
    }

    private readonly ConcurrentDictionary<(Guid Person, Guid Album), bool> _takingPart = new();

    /// <summary>In the album, not only reading it through a link.</summary>
    public void TakesPart(Guid personId, Guid albumId)
    {
        _visible[(personId, albumId)] = true;
        _takingPart[(personId, albumId)] = true;
    }

    private readonly ConcurrentDictionary<(Guid Person, Guid Owner), bool> _legacy = new();

    public void IsLegacyContact(Guid personId, Guid ownerAccountId) => _legacy[(personId, ownerAccountId)] = true;

    public Task<IReadOnlyCollection<Guid>> LegacyArchivesAsync(Guid personId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>([.. _legacy.Keys.Where(key => key.Person == personId).Select(key => key.Owner)]);

    public Task<IReadOnlyCollection<Guid>> AlbumsTakingPartInAsync(Guid personId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>([.. _takingPart.Keys.Where(key => key.Person == personId).Select(key => key.Album)]);

    public Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_visible.ContainsKey((personId, albumId)));

    public Task<IReadOnlyCollection<Guid>> VisibleAlbumsAsync(Guid personId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>([.. _visible.Keys.Where(key => key.Person == personId).Select(key => key.Album)]);

    public Task<AlbumPoolState> GetStateAsync(Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(new AlbumPoolState(0, long.MaxValue));

    private readonly ConcurrentDictionary<Guid, bool> _withoutTerms = new();
    private readonly ConcurrentDictionary<Guid, bool> _fullPools = new();
    private readonly ConcurrentDictionary<Guid, long> _reserved = new();
    private readonly ConcurrentDictionary<Guid, Guid> _payers = new();

    /// <summary>Which pool pays for an operation still reserved.</summary>
    public Guid? PayerOf(Guid operationKey) => _reserved.ContainsKey(operationKey) ? _payers[operationKey] : null;

    public Task ReserveOverLimitAsync(Guid accountId, Guid operationKey, long bytes, CancellationToken cancellationToken)
    {
        _reserved[operationKey] = bytes;
        _payers[operationKey] = accountId;
        return Task.CompletedTask;
    }

    /// <summary>Space reserved and not given back, by operation.</summary>
    public IReadOnlyDictionary<Guid, long> Reserved => _reserved;

    public void WithFullPool(Guid accountId) => _fullPools[accountId] = true;

    public Task<bool> TryReserveAsync(Guid accountId, Guid operationKey, long bytes, CancellationToken cancellationToken)
    {
        if (_fullPools.ContainsKey(accountId))
        {
            return Task.FromResult(false);
        }

        _reserved[operationKey] = bytes;
        _payers[operationKey] = accountId;
        return Task.FromResult(true);
    }

    public Task ReleaseAsync(Guid operationKey, CancellationToken cancellationToken)
    {
        _reserved.TryRemove(operationKey, out _);
        return Task.CompletedTask;
    }

    public void WithoutTerms(Guid accountId) => _withoutTerms[accountId] = true;

    public Task<PlaceInfo?> FindAsync(Guid accountId, string placeReference, CancellationToken cancellationToken) =>
        Task.FromResult(placeReference == MissingPlace ? null : new PlaceInfo(placeReference, "Giewont", 49.25, 19.93));

    public Task<bool> HasAcceptedCurrentTermsAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult(!_withoutTerms.ContainsKey(accountId));
}

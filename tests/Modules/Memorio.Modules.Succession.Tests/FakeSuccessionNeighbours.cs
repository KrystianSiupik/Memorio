using System.Collections.Concurrent;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.SharedKernel.Scheduling;
using Memorio.SharedKernel.Storage;

namespace Memorio.Modules.Succession.Tests;

/// <summary>The other modules Succession asks, and what it calls on them.</summary>
public sealed class FakeSuccessionNeighbours : IAlbumMembership, IAlbumNames, IAlbumSuccession, IAccountDirectory, IObjectStorage, IDeadlines
{
    private readonly ConcurrentDictionary<Guid, (Guid Owner, Guid? Successor, HashSet<Guid> Members)> _albums = new();
    private readonly ConcurrentDictionary<Guid, string> _emails = new();
    private readonly ConcurrentDictionary<string, bool> _uploads = new();
    private readonly ConcurrentDictionary<(string Job, string Key), DateTimeOffset> _deadlines = new();

    public ConcurrentBag<string> Deleted { get; } = [];

    public ConcurrentBag<string> Effects { get; } = [];

    public Guid NewAccount()
    {
        var id = Guid.CreateVersion7();
        _emails[id] = $"{id}@example.invalid";
        return id;
    }

    public string EmailOf(Guid accountId) => _emails[accountId];

    public (Guid Album, Guid Owner, Guid Member) NewAlbum(bool withSuccessor = true, Guid? owner = null)
    {
        var (album, ownerId, member) = (Guid.CreateVersion7(), owner ?? NewAccount(), NewAccount());
        _albums[album] = (ownerId, withSuccessor ? member : null, [ownerId, member]);
        return (album, ownerId, member);
    }

    public void ChangeSuccessor(Guid album, Guid? successor) => _albums[album] = _albums[album] with { Successor = successor };

    public string AddUpload()
    {
        var key = $"uploads/{Guid.NewGuid():N}";
        _uploads[key] = true;
        return key;
    }

    public DateTimeOffset? DeadlineOf(string job, string key) => _deadlines.TryGetValue((job, key), out var at) ? at : null;

    public Task<bool> IsMemberAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_albums.TryGetValue(albumId, out var album) && album.Members.Contains(accountId));

    public Task<Guid?> OwnerOfAsync(Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_albums.TryGetValue(albumId, out var album) ? album.Owner : (Guid?)null);

    public Task<IReadOnlyCollection<Guid>> AlbumsOfAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>([.. _albums.Where(album => album.Value.Members.Contains(accountId)).Select(album => album.Key)]);

    public Task<IReadOnlyDictionary<Guid, string>> NamesOfAsync(IReadOnlyCollection<Guid> albumIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<Guid, string>>(albumIds.ToDictionary(id => id, _ => "Rodzina"));

    public Task<SuccessorOutcome> NameAsync(Guid albumId, Guid ownerAccountId, Guid successorAccountId, CancellationToken cancellationToken) =>
        Task.FromResult(SuccessorOutcome.Named);

    public Task<IReadOnlyList<OwnedAlbum>> OwnedByAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<OwnedAlbum>>([.. _albums.Where(album => album.Value.Owner == accountId).Select(album => new OwnedAlbum(album.Key, album.Value.Successor))]);

    public Task TransferAsync(Guid albumId, Guid fromAccountId, Guid toAccountId, CancellationToken cancellationToken)
    {
        Effects.Add($"transfer:{albumId}:{toAccountId}");
        return Task.CompletedTask;
    }

    public Task ReleaseOwnerAsync(Guid albumId, Guid ownerAccountId, CancellationToken cancellationToken)
    {
        Effects.Add($"release:{albumId}");
        return Task.CompletedTask;
    }

    public Task<int> MemoriesOfAsync(Guid albumId, Guid authorAccountId, CancellationToken cancellationToken) => Task.FromResult(3);

    public Task<string?> EmailOfAsync(Guid accountId, CancellationToken cancellationToken) => Task.FromResult(_emails.GetValueOrDefault(accountId));

    public Task<IReadOnlyDictionary<Guid, string>> EmailsOfAsync(IReadOnlyCollection<Guid> accountIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<Guid, string>>(accountIds.Where(_emails.ContainsKey).ToDictionary(id => id, id => _emails[id]));

    public Task<IReadOnlyDictionary<StorageKey, SignedUrl>> SignReadsAsync(IReadOnlyCollection<StorageKey> keys, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task<UploadSlot> CreateUploadAsync(string contentType, CancellationToken cancellationToken) =>
        Task.FromResult(new UploadSlot(new StorageKey(AddUpload()), new SignedUrl(new Uri("https://storage.example.invalid/upload"), DateTimeOffset.UtcNow.AddMinutes(5)), contentType));

    public Task<StoredObject?> PromoteUploadAsync(StorageKey upload, StorageKey destination, CancellationToken cancellationToken) =>
        Task.FromResult(_uploads.TryRemove(upload.Value, out _) ? new StoredObject(destination, 100, "image/jpeg") : null);

    public Task DeleteAsync(StorageKey key, CancellationToken cancellationToken)
    {
        Deleted.Add(key.Value);
        return Task.CompletedTask;
    }

    public Task ScheduleAsync(string job, string key, DateTimeOffset dueAt, IReadOnlyDictionary<string, string>? data, CancellationToken cancellationToken)
    {
        _deadlines[(job, key)] = dueAt;
        return Task.CompletedTask;
    }

    public Task CancelAsync(string job, string key, CancellationToken cancellationToken)
    {
        _deadlines.TryRemove((job, key), out _);
        return Task.CompletedTask;
    }
}

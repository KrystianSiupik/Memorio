using System.Collections.Concurrent;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;

namespace Memorio.Modules.Access.Tests;

/// <summary>What Album and Account answer: albums with owners and participants, from memory.</summary>
public sealed class FakeAccessNeighbours : IAlbumMembership, IAlbumNames, IAccountDirectory
{
    private readonly ConcurrentDictionary<Guid, Guid> _owners = new();
    private readonly ConcurrentDictionary<(Guid Album, Guid Account), bool> _participants = new();

    public (Guid Album, Guid Owner) NewAlbum()
    {
        var (album, owner) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        _owners[album] = owner;
        return (album, owner);
    }

    public Guid NewParticipant(Guid album)
    {
        var person = Guid.CreateVersion7();
        _participants[(album, person)] = true;
        return person;
    }

    public void AddParticipant(Guid album, Guid person) => _participants[(album, person)] = true;

    public void Leaves(Guid album, Guid person) => _participants.TryRemove((album, person), out _);

    public Task<bool> IsMemberAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_owners.GetValueOrDefault(albumId) == accountId || _participants.ContainsKey((albumId, accountId)));

    public Task<Guid?> OwnerOfAsync(Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_owners.TryGetValue(albumId, out var owner) ? owner : (Guid?)null);

    public Task<IReadOnlyCollection<Guid>> AlbumsOfAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>(
            [.. _owners.Where(item => item.Value == accountId).Select(item => item.Key).Union(_participants.Keys.Where(key => key.Account == accountId).Select(key => key.Album))]);

    public Task<IReadOnlyDictionary<Guid, string>> NamesOfAsync(IReadOnlyCollection<Guid> albumIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<Guid, string>>(albumIds.Where(_owners.ContainsKey).ToDictionary(id => id, _ => "Rodzina"));

    public Task<string?> EmailOfAsync(Guid accountId, CancellationToken cancellationToken) => Task.FromResult<string?>($"{accountId}@example.invalid");

    public Task<IReadOnlyDictionary<Guid, string>> EmailsOfAsync(IReadOnlyCollection<Guid> accountIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<Guid, string>>(accountIds.ToDictionary(id => id, id => $"{id}@example.invalid"));
}

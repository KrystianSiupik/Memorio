using System.Collections.Concurrent;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Archive.Contracts;

namespace Memorio.Modules.Moderation.Tests;

/// <summary>What Moderation asks of Archive, Album and Access: who sees what, and whose it is.</summary>
public sealed class FakeModerationNeighbours : IAlbumMembership, IAccessReader, IReportedMemories, IAlbumContent, IContentHiding
{
    private readonly ConcurrentDictionary<Guid, HiddenContent> _hidden = new();

    /// <summary>What Access was asked to hide, by hold.</summary>
    public IReadOnlyDictionary<Guid, HiddenContent> Hidden => _hidden;

    /// <summary>Access does not answer: the order is not confirmed.</summary>
    public bool AccessDown { get; set; }

    public Task<bool> HideAsync(HiddenContent content, CancellationToken cancellationToken)
    {
        if (AccessDown)
        {
            return Task.FromResult(false);
        }

        _hidden[content.HoldId] = content;
        return Task.FromResult(true);
    }

    public Task<bool> ReleaseAsync(Guid holdId, Guid resourceId, int sequence, CancellationToken cancellationToken) => Task.FromResult(_hidden.TryRemove(holdId, out _));

    public Task<AlbumMemoryFacts?> DescribeAsync(Guid memoryId, CancellationToken cancellationToken) =>
        Task.FromResult(_memories.TryGetValue(memoryId, out var memory) ? new AlbumMemoryFacts(memory.Author, 0) : null);

    public Task<IReadOnlyDictionary<Guid, AlbumMemorySummary>> SummarizeAsync(IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task LinkAsync(Guid memoryId, Guid albumId, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task UnlinkAsync(Guid memoryId, Guid albumId, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task<IReadOnlyList<Guid>> OwnInPeriodAsync(Guid accountId, DateOnly from, DateOnly to, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task<IReadOnlyList<Guid>> ChooseToTrimAsync(Guid albumId, IReadOnlyCollection<Guid> keptMemoryIds, long limitBytes, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task<IReadOnlyList<AlbumTrimMaterial>> TrimMaterialAsync(Guid albumId, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task<AlbumPhotoOutcome> AddPhotoAsync(AlbumPhoto photo, CancellationToken cancellationToken) => throw new NotSupportedException();

    private readonly ConcurrentDictionary<Guid, (Guid Author, HashSet<Guid> Seen)> _memories = new();
    private readonly ConcurrentDictionary<Guid, (Guid Owner, HashSet<Guid> Seen)> _albums = new();

    /// <summary>A memory of a new author, seen by a new reader.</summary>
    public (Guid Memory, Guid Author, Guid Reader) NewMemory()
    {
        var (memory, author, reader) = (Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        _memories[memory] = (author, [author, reader]);
        return (memory, author, reader);
    }

    /// <summary>One more person sees this memory.</summary>
    public void Sees(Guid memory, Guid person) => _memories[memory].Seen.Add(person);

    public (Guid Album, Guid Owner, Guid Member) NewAlbum()
    {
        var (album, owner, member) = (Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        _albums[album] = (owner, [owner, member]);
        return (album, owner, member);
    }

    public Task<Guid?> AuthorSeenByAsync(Guid memoryId, Guid personId, CancellationToken cancellationToken) =>
        Task.FromResult(_memories.TryGetValue(memoryId, out var memory) && memory.Seen.Contains(personId) ? memory.Author : (Guid?)null);

    public Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_albums.TryGetValue(albumId, out var album) && album.Seen.Contains(personId));

    public Task<Guid?> OwnerOfAsync(Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_albums.TryGetValue(albumId, out var album) ? album.Owner : (Guid?)null);

    public Task<bool> IsMemberAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken) => CanSeeAlbumAsync(accountId, albumId, cancellationToken);

    public Task<IReadOnlyCollection<Guid>> AlbumsOfAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>([.. _albums.Where(album => album.Value.Seen.Contains(accountId)).Select(album => album.Key)]);

    public Task<IReadOnlyCollection<Guid>> VisibleAlbumsAsync(Guid personId, CancellationToken cancellationToken) => AlbumsOfAsync(personId, cancellationToken);

    public Task<IReadOnlyCollection<Guid>> AlbumsTakingPartInAsync(Guid personId, CancellationToken cancellationToken) => AlbumsOfAsync(personId, cancellationToken);

    public Task<IReadOnlyCollection<Guid>> LegacyArchivesAsync(Guid personId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<Guid>>([]);

    public Task<IReadOnlyCollection<Guid>> HiddenFromAsync(Guid personId, IReadOnlyCollection<Guid> resourceIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>([]);
}

using System.Collections.Concurrent;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Notifications.Contracts;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Album.Tests;

/// <summary>The other modules Album asks.</summary>
public sealed class FakeAlbumNeighbours : IAlbumSeatLimit, IInvitationBlocks, IAccountDirectory, IAlbumContent, IAlbumPool
{
    private readonly ConcurrentDictionary<Guid, int> _seats = new();
    private readonly ConcurrentDictionary<string, bool> _blocked = new();
    private readonly ConcurrentDictionary<Guid, string> _emails = new();
    private readonly ConcurrentDictionary<Guid, AlbumMemoryFacts> _memories = new();
    private readonly ConcurrentDictionary<Guid, bool> _fullPools = new();
    private readonly ConcurrentDictionary<Guid, long> _reserved = new();
    private readonly ConcurrentDictionary<(Guid Memory, Guid Album), bool> _links = new();
    private readonly ConcurrentDictionary<Guid, AlbumPhotoOutcome> _photoOutcomes = new();

    public IReadOnlyDictionary<Guid, long> Reserved => _reserved;

    public void SeatsOf(Guid albumId, int seats) => _seats[albumId] = seats;

    public void Block(string email) => _blocked[email] = true;

    public Guid NewAccount(string? email = null)
    {
        var id = Guid.CreateVersion7();
        _emails[id] = email ?? $"{id}@example.invalid";
        return id;
    }

    public string EmailOf(Guid accountId) => _emails[accountId];

    public Guid NewMemory(Guid authorAccountId, long sizeBytes = 100)
    {
        var id = Guid.CreateVersion7();
        _memories[id] = new AlbumMemoryFacts(authorAccountId, sizeBytes);
        return id;
    }

    public void WithFullPool(Guid albumId) => _fullPools[albumId] = true;

    public void NextPhotoIn(Guid memoryId, AlbumPhotoOutcome outcome) => _photoOutcomes[memoryId] = outcome;

    public bool IsLinked(Guid memoryId, Guid albumId) => _links.ContainsKey((memoryId, albumId));

    public Task<int> SeatsOfAsync(Guid albumId, CancellationToken cancellationToken) => Task.FromResult(_seats.GetValueOrDefault(albumId, 10));

    public Task<bool> IsBlockedAsync(string email, CancellationToken cancellationToken) => Task.FromResult(_blocked.ContainsKey(email));

    public Task<string?> EmailOfAsync(Guid accountId, CancellationToken cancellationToken) => Task.FromResult(_emails.GetValueOrDefault(accountId));

    public Task<IReadOnlyDictionary<Guid, string>> EmailsOfAsync(IReadOnlyCollection<Guid> accountIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<Guid, string>>(accountIds.Where(_emails.ContainsKey).ToDictionary(id => id, id => _emails[id]));

    public Task<AlbumMemoryFacts?> DescribeAsync(Guid memoryId, CancellationToken cancellationToken) => Task.FromResult(_memories.GetValueOrDefault(memoryId));

    public Task<IReadOnlyDictionary<Guid, AlbumMemorySummary>> SummarizeAsync(IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<Guid, AlbumMemorySummary>>(memoryIds.Where(_memories.ContainsKey).ToDictionary(id => id, _ => new AlbumMemorySummary("Giewont", "2026-07-01", 0)));

    public Task LinkAsync(Guid memoryId, Guid albumId, CancellationToken cancellationToken)
    {
        _links[(memoryId, albumId)] = true;
        return Task.CompletedTask;
    }

    public Task UnlinkAsync(Guid memoryId, Guid albumId, CancellationToken cancellationToken)
    {
        _links.TryRemove((memoryId, albumId), out _);
        return Task.CompletedTask;
    }

    public Task<AlbumPhotoOutcome> AddPhotoAsync(AlbumPhoto photo, CancellationToken cancellationToken) =>
        Task.FromResult(_photoOutcomes.GetValueOrDefault(photo.MemoryId, AlbumPhotoOutcome.Added));

    public Task<bool> TryReserveAsync(Guid albumId, Guid operationKey, long bytes, CancellationToken cancellationToken)
    {
        if (_fullPools.ContainsKey(albumId))
        {
            return Task.FromResult(false);
        }

        _reserved[operationKey] = bytes;
        return Task.FromResult(true);
    }

    public Task ReleaseAsync(Guid operationKey, CancellationToken cancellationToken)
    {
        _reserved.TryRemove(operationKey, out _);
        return Task.CompletedTask;
    }

    public Task<AlbumPoolState> GetStateAsync(Guid albumId, CancellationToken cancellationToken) => Task.FromResult(new AlbumPoolState(0, 1000));
}

/// <summary>Holds deadlines the way the scheduler does: one per job and key, a later one replaces it.</summary>
public sealed class FakeDeadlines : IDeadlines
{
    private readonly ConcurrentDictionary<(string Job, string Key), ScheduledJobRun> _deadlines = new();

    public ScheduledJobRun? Find(string job, string key) => _deadlines.GetValueOrDefault((job, key));

    public Task ScheduleAsync(string job, string key, DateTimeOffset dueAt, IReadOnlyDictionary<string, string>? data, CancellationToken cancellationToken)
    {
        _deadlines[(job, key)] = new ScheduledJobRun(job, key, dueAt, data ?? new Dictionary<string, string>(), Attempt: 1);
        return Task.CompletedTask;
    }

    public Task CancelAsync(string job, string key, CancellationToken cancellationToken)
    {
        _deadlines.TryRemove((job, key), out _);
        return Task.CompletedTask;
    }
}

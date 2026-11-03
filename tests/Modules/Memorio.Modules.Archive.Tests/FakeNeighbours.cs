using System.Collections.Concurrent;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Places.Contracts;

namespace Memorio.Modules.Archive.Tests;

/// <summary>The other modules Archive asks: every place exists but "missing", everybody accepted the terms unless told otherwise.</summary>
public sealed class FakeNeighbours : IPlaceReader, ITermsReader, ISpacePool
{
    public const string MissingPlace = "osm:N:404";

    private readonly ConcurrentDictionary<Guid, bool> _withoutTerms = new();
    private readonly ConcurrentDictionary<Guid, bool> _fullPools = new();
    private readonly ConcurrentDictionary<Guid, long> _reserved = new();

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

using System.Collections.Concurrent;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Places.Contracts;

namespace Memorio.Modules.Archive.Tests;

/// <summary>The other modules Archive asks: every place exists but "missing", everybody accepted the terms unless told otherwise.</summary>
public sealed class FakeNeighbours : IPlaceReader, ITermsReader
{
    public const string MissingPlace = "osm:N:404";

    private readonly ConcurrentDictionary<Guid, bool> _withoutTerms = new();

    public void WithoutTerms(Guid accountId) => _withoutTerms[accountId] = true;

    public Task<PlaceInfo?> FindAsync(Guid accountId, string placeReference, CancellationToken cancellationToken) =>
        Task.FromResult(placeReference == MissingPlace ? null : new PlaceInfo(placeReference, "Giewont", 49.25, 19.93));

    public Task<bool> HasAcceptedCurrentTermsAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult(!_withoutTerms.ContainsKey(accountId));
}

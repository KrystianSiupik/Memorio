using Memorio.Modules.Places.Domain.Places;

namespace Memorio.Modules.Places.Application.Queries.PlaceLookup;

/// <summary>A place known by its reference; an own place exists only for its owner.</summary>
internal interface IPlaceLookup
{
    Task<FoundPlace?> FindAsync(Guid accountId, PlaceReference reference, CancellationToken cancellationToken);
}

internal sealed record FoundPlace(string PlaceReference, string Name, double Latitude, double Longitude, bool IsOwn);

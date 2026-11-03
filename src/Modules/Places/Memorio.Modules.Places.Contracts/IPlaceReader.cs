namespace Memorio.Modules.Places.Contracts;

/// <summary>Read port for Archive: whether a place exists for this person, and what a memory keeps of it.</summary>
public interface IPlaceReader
{
    /// <summary>Null when the reference is not a place this person can use.</summary>
    Task<PlaceInfo?> FindAsync(Guid accountId, string placeReference, CancellationToken cancellationToken);
}

public sealed record PlaceInfo(string PlaceReference, string Name, double Latitude, double Longitude);

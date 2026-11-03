namespace Memorio.Modules.Places.Application.Queries.PlaceSuggestions;

/// <summary>A place to choose: from OpenStreetMap, or one of the person's own.</summary>
internal sealed record PlaceSuggestion(string PlaceReference, string Name, double Latitude, double Longitude, bool IsOwn);

/// <summary>R3: places whose name starts with what the person typed. Own places are the asker's only.</summary>
internal interface IPlaceSuggestionsQuery
{
    Task<IReadOnlyList<PlaceSuggestion>> FindAsync(Guid accountId, string? text, int limit, CancellationToken cancellationToken);
}

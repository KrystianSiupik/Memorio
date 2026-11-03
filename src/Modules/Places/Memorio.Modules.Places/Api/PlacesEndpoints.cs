using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Places.Application.Queries.PlaceSuggestions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Places.Api;

/// <summary>Choosing where a memory happened.</summary>
internal static class PlacesEndpoints
{
    public static void MapPlacesEndpoints(this IEndpointRouteBuilder v1)
    {
        var places = v1.MapGroup("/places").WithTags("Places");

        places.MapGet("/suggestions", GetSuggestionsAsync)
            .WithName("GetPlaceSuggestions")
            .WithSummary("Places whose name starts with the text: the person's own first, then OpenStreetMap.");
    }

    private static async Task<Ok<IReadOnlyList<PlaceResponse>>> GetSuggestionsAsync(
        string? text,
        int? limit,
        ClaimsPrincipal principal,
        IPlaceSuggestionsQuery query,
        CancellationToken cancellationToken)
    {
        var found = await query.FindAsync(CurrentAccount.IdOf(principal), text, limit ?? 10, cancellationToken);
        return TypedResults.Ok<IReadOnlyList<PlaceResponse>>(
            [.. found.Select(place => new PlaceResponse(place.PlaceReference, place.Name, place.Latitude, place.Longitude, place.IsOwn))]);
    }
}

/// <param name="PlaceReference">Pass it on to choose this place.</param>
internal sealed record PlaceResponse(string PlaceReference, string Name, double Latitude, double Longitude, bool IsOwn);

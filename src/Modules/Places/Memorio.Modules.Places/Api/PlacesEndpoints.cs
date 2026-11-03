using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Places.Application.Commands.ChoosePlace;
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
        places.MapPost("/choice", ChooseAsync)
            .WithName("ChoosePlace")
            .WithSummary("Checks the place a person chose from the suggestions and answers with it.");
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

    private static async Task<Results<Ok<PlaceResponse>, ProblemHttpResult>> ChooseAsync(
        ChoosePlaceRequest request,
        ClaimsPrincipal principal,
        ChoosePlaceHandler handler,
        CancellationToken cancellationToken)
    {
        var chosen = await handler.HandleAsync(new ChoosePlace(CurrentAccount.IdOf(principal), request.PlaceReference), cancellationToken);
        if (chosen.IsFailure)
        {
            return Problems.From(chosen.Error);
        }

        var place = chosen.Value.Place;
        return TypedResults.Ok(new PlaceResponse(place.PlaceReference, place.Name, place.Latitude, place.Longitude, place.IsOwn));
    }
}

internal sealed record ChoosePlaceRequest(string? PlaceReference);

/// <param name="PlaceReference">Pass it on to choose this place.</param>
internal sealed record PlaceResponse(string PlaceReference, string Name, double Latitude, double Longitude, bool IsOwn);

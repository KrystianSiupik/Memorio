using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Places.Application.Commands.AddOwnPlace;
using Memorio.Modules.Places.Application.Commands.ChoosePlace;
using Memorio.Modules.Places.Application.Queries.PlaceSuggestions;
using Memorio.Modules.Places.Domain.Places;
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
        places.MapPost("/own", AddOwnAsync)
            .WithName("AddOwnPlace")
            .WithSummary("Adds a place of the signed-in person's own; the id comes from the client, so a retry changes nothing.");
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

    private static async Task<Results<Ok<PlaceResponse>, ProblemHttpResult>> AddOwnAsync(
        AddOwnPlaceRequest request,
        ClaimsPrincipal principal,
        AddOwnPlaceHandler handler,
        CancellationToken cancellationToken)
    {
        var added = await handler.HandleAsync(
            new AddOwnPlace(request.Id, CurrentAccount.IdOf(principal), request.Name, request.Latitude, request.Longitude),
            cancellationToken);
        if (added.IsFailure)
        {
            return Problems.From(added.Error);
        }

        var place = added.Value.Place;
        return TypedResults.Ok(new PlaceResponse(PlaceReference.Own(place.Id), place.Name, place.Latitude, place.Longitude, IsOwn: true));
    }
}

internal sealed record ChoosePlaceRequest(string? PlaceReference);

/// <param name="Id">A UUID the client made for this place; the idempotency key.</param>
internal sealed record AddOwnPlaceRequest(Guid Id, string? Name, double Latitude, double Longitude);

/// <param name="PlaceReference">Pass it on to choose this place.</param>
internal sealed record PlaceResponse(string PlaceReference, string Name, double Latitude, double Longitude, bool IsOwn);

using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.AiNarration.Application.Commands.RequestDraft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.AiNarration.Api;

internal static class DraftsEndpoints
{
    public static void MapDraftsEndpoints(this IEndpointRouteBuilder v1)
    {
        v1.MapPost("/ai/drafts", RequestAsync)
            .WithTags("AI")
            .WithName("RequestAiDraft")
            .WithSummary("Writes a draft of a description with the AI; the id comes from the client, so a retry gets the same draft.");
    }

    private static async Task<Results<Ok<AiDraftResponse>, ProblemHttpResult>> RequestAsync(
        RequestAiDraftRequest request,
        ClaimsPrincipal principal,
        RequestDraftHandler handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<(bool IsPerson, string? Name)> people = (request.PeopleNames ?? []).Select(name => (true, (string?)name));
        var places = (request.PlaceNames ?? []).Select(name => (false, (string?)name));
        var draft = await handler.HandleAsync(
            new RequestDraft(request.Id, CurrentAccount.IdOf(principal), request.Material, request.ConsentsToAi, [.. people.Concat(places)]),
            cancellationToken);
        return draft.IsFailure
            ? Problems.From(draft.Error)
            : TypedResults.Ok(new AiDraftResponse(draft.Value.Id, draft.Value.Text, IsAiGenerated: true));
    }
}

/// <param name="Id">A UUID the client made for this draft; the idempotency key.</param>
/// <param name="Material">What the person wrote so far.</param>
/// <param name="ConsentsToAi">The person agreed that the material goes to the AI provider, without these names.</param>
/// <param name="PeopleNames">Names of people in the material.</param>
/// <param name="PlaceNames">Names of places in the material.</param>
internal sealed record RequestAiDraftRequest(Guid Id, string? Material, bool ConsentsToAi, IReadOnlyList<string>? PeopleNames, IReadOnlyList<string>? PlaceNames);

/// <param name="IsAiGenerated">Always true: the marker shown with this text.</param>
internal sealed record AiDraftResponse(Guid Id, string Text, bool IsAiGenerated);

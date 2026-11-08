using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.AiNarration.Application.Commands.Consents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.AiNarration.Api;

internal static class ConsentEndpoints
{
    public static void MapConsentEndpoints(this IEndpointRouteBuilder v1)
    {
        v1.MapGet("/albums/{albumId:guid}/ai-consent", GetAsync)
            .WithTags("AI")
            .WithName("GetAiConsent")
            .WithSummary("Whether the signed-in person lets the AI use their content in this album (S2:C5).");
        v1.MapPut("/albums/{albumId:guid}/ai-consent", GiveAsync)
            .WithTags("AI")
            .WithName("GiveAiConsent")
            .WithSummary("C80: the participant lets the AI use their content in this album; again changes nothing.");
    }

    private static async Task<Ok<AiConsentResponse>> GetAsync(Guid albumId, ClaimsPrincipal principal, AiConsentQuery query, CancellationToken cancellationToken) =>
        TypedResults.Ok(new AiConsentResponse(await query.IsGivenAsync(CurrentAccount.IdOf(principal), albumId, cancellationToken)));

    private static async Task<Results<NoContent, ProblemHttpResult>> GiveAsync(
        Guid albumId,
        ClaimsPrincipal principal,
        GiveAiConsentHandler handler,
        CancellationToken cancellationToken)
    {
        var given = await handler.HandleAsync(new GiveAiConsent(CurrentAccount.IdOf(principal), albumId), cancellationToken);
        return given.IsFailure ? Problems.From(given.Error) : TypedResults.NoContent();
    }
}

internal sealed record AiConsentResponse(bool Given);

using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.AiNarration.Application.Commands.Summaries;
using Memorio.Modules.AiNarration.Application.Queries.AlbumSummaries;
using Memorio.Modules.AiNarration.Domain.Summaries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.AiNarration.Api;

internal static class SummaryEndpoints
{
    public static void MapSummaryEndpoints(this IEndpointRouteBuilder v1)
    {
        v1.MapPost("/ai/summaries", RequestAsync)
            .WithTags("AI")
            .WithName("RequestAiSummary")
            .WithSummary("C82: the AI's story of a period of an album, from the materials the person may use; the id comes from the client.");
        v1.MapGet("/ai/summaries/{id:guid}", GetAsync)
            .WithTags("AI")
            .WithName("GetAiSummary")
            .WithSummary("One summary of the person who asked for it.");
        v1.MapPost("/ai/summaries/{id:guid}/save", SaveAsync)
            .WithTags("AI")
            .WithName("SaveAiSummary")
            .WithSummary("C83: the person approves the summary as it is and keeps it; the memories it tells of are checked again.");
        v1.MapPost("/ai/summaries/{id:guid}/album", AddToAlbumAsync)
            .WithTags("AI")
            .WithName("AddAiSummaryToAlbum")
            .WithSummary("C84: a saved summary shown to the participants of its album.");
        v1.MapGet("/albums/{albumId:guid}/ai-summaries", InAlbumAsync)
            .WithTags("AI")
            .WithName("ListAlbumAiSummaries")
            .WithSummary("The summaries added to the album that this participant may read every memory of, the newest first.");
    }

    private static async Task<Results<Ok<AiSummaryResponse>, ProblemHttpResult>> RequestAsync(
        RequestAiSummaryRequest request,
        ClaimsPrincipal principal,
        RequestSummaryHandler handler,
        CancellationToken cancellationToken)
    {
        if (!DateOnly.TryParseExact(request.From, "yyyy-MM-dd", out var from) || !DateOnly.TryParseExact(request.To, "yyyy-MM-dd", out var to))
        {
            return Problems.From(SummaryErrors.InvalidPeriod);
        }

        var summary = await handler.HandleAsync(new RequestSummary(request.Id, CurrentAccount.IdOf(principal), request.AlbumId, from, to), cancellationToken);
        return summary.IsFailure ? Problems.From(summary.Error) : TypedResults.Ok(AiSummaryResponse.Of(summary.Value));
    }

    private static async Task<Results<Ok<AiSummaryResponse>, ProblemHttpResult>> SaveAsync(
        Guid id,
        ClaimsPrincipal principal,
        ApproveSummaryHandler handler,
        CancellationToken cancellationToken)
    {
        var summary = await handler.HandleAsync(new ApproveSummary(id, CurrentAccount.IdOf(principal)), cancellationToken);
        return summary.IsFailure ? Problems.From(summary.Error) : TypedResults.Ok(AiSummaryResponse.Of(summary.Value));
    }

    private static async Task<Results<Ok<AiSummaryResponse>, ProblemHttpResult>> AddToAlbumAsync(
        Guid id,
        ClaimsPrincipal principal,
        AddSummaryToAlbumHandler handler,
        CancellationToken cancellationToken)
    {
        var summary = await handler.HandleAsync(new AddSummaryToAlbum(id, CurrentAccount.IdOf(principal)), cancellationToken);
        return summary.IsFailure ? Problems.From(summary.Error) : TypedResults.Ok(AiSummaryResponse.Of(summary.Value));
    }

    private static async Task<Results<Ok<IReadOnlyList<AiSummaryResponse>>, ProblemHttpResult>> InAlbumAsync(
        Guid albumId,
        ClaimsPrincipal principal,
        AlbumSummariesQuery query,
        CancellationToken cancellationToken)
    {
        var summaries = await query.ForAsync(CurrentAccount.IdOf(principal), albumId, cancellationToken);
        return summaries.IsFailure
            ? Problems.From(summaries.Error)
            : TypedResults.Ok<IReadOnlyList<AiSummaryResponse>>([.. summaries.Value.Select(AiSummaryResponse.Of)]);
    }

    private static async Task<Results<Ok<AiSummaryResponse>, ProblemHttpResult>> GetAsync(
        Guid id,
        ClaimsPrincipal principal,
        IPeriodSummaries summaries,
        CancellationToken cancellationToken) =>
        await summaries.FindAsync(id, cancellationToken) is { } summary && summary.AccountId == CurrentAccount.IdOf(principal)
            ? TypedResults.Ok(AiSummaryResponse.Of(summary))
            : Problems.From(SummaryErrors.NotFound);
}

/// <param name="Id">A UUID the client made for this summary; the idempotency key.</param>
/// <param name="From">yyyy-MM-dd.</param>
/// <param name="To">yyyy-MM-dd.</param>
internal sealed record RequestAiSummaryRequest(Guid Id, Guid AlbumId, string? From, string? To);

/// <param name="AccountId">Who asked for it.</param>
/// <param name="State">Draft, Saved or Invalidated; an invalidated summary has no text any more.</param>
/// <param name="IsAiGenerated">Always true: the marker shown with this text.</param>
/// <param name="LeftOutMemories">Memories of the period not used, because their authors gave no AI consent.</param>
/// <param name="LeftOutPeople">How many people those memories belong to.</param>
/// <param name="DroppedSentences">Sentences of the AI dropped for pointing to no memory.</param>
internal sealed record AiSummaryResponse(
    Guid Id,
    Guid AccountId,
    Guid AlbumId,
    string From,
    string To,
    string State,
    string Text,
    IReadOnlyList<AiSummarySentence> Sentences,
    bool IsAiGenerated,
    string Model,
    string PromptVersion,
    DateTimeOffset GeneratedAt,
    int LeftOutMemories,
    int LeftOutPeople,
    int DroppedSentences,
    bool AddedToAlbum,
    string? InvalidReason)
{
    public static AiSummaryResponse Of(PeriodSummary summary) => new(
        summary.Id,
        summary.AccountId,
        summary.AlbumId,
        summary.From.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
        summary.To.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
        summary.State.ToString(),
        summary.Text,
        [.. summary.Sentences.Select(sentence => new AiSummarySentence(sentence.Text, sentence.MemoryIds))],
        true,
        summary.Model,
        summary.PromptVersion,
        summary.GeneratedAt,
        summary.LeftOutMemories,
        summary.LeftOutPeople,
        summary.DroppedSentences,
        summary.AddedToAlbumAt is not null,
        summary.InvalidReason);
}

/// <param name="MemoryIds">The memories this sentence tells of.</param>
internal sealed record AiSummarySentence(string Text, IReadOnlyList<Guid> MemoryIds);

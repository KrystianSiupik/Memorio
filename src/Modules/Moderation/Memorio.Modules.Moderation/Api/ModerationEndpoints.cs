using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Moderation.Application.Commands.Reports;
using Memorio.Modules.Moderation.Application.Queries;
using Memorio.Modules.Moderation.Domain.Reports;
using Memorio.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Moderation.Api;

internal static class ModerationEndpoints
{
    public static void MapModerationEndpoints(this IEndpointRouteBuilder v1)
    {
        var reports = v1.MapGroup("/reports").WithTags("Moderation");
        reports.MapPost("/", ReportAsync).WithName("ReportContent").WithSummary("Reports a memory or an album the person sees; the id comes from the client, and the same report again changes nothing.");
        reports.MapPost("/{id:guid}/appeal", AppealAsync).WithName("AppealDecision").WithSummary("The author appeals against the removal of their content, with reasons.");

        v1.MapGet("/moderation/mine", MineAsync)
            .WithTags("Moderation")
            .WithName("GetMyModeration")
            .WithSummary("What the person reported, and the decisions about their own content, with the reasons.");
    }

    private static async Task<Results<Ok<ReportedResponse>, ProblemHttpResult>> ReportAsync(ReportRequest request, ClaimsPrincipal principal, SubmitReportHandler handler, CancellationToken cancellationToken)
    {
        var reported = await handler.HandleAsync(
            new SubmitReport(new ReportId(request.Id), CurrentAccount.IdOf(principal), request.Target, request.TargetId, request.Reason, request.Explanation),
            cancellationToken);
        return reported.IsFailure ? Problems.From(reported.Error) : TypedResults.Ok(new ReportedResponse(reported.Value.Value));
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> AppealAsync(Guid id, AppealRequest request, ClaimsPrincipal principal, ReportDecisionHandler handler, CancellationToken cancellationToken)
    {
        var appealed = await handler.HandleAsync(new AppealDecision(new ReportId(id), CurrentAccount.IdOf(principal), request.Text), cancellationToken);
        return appealed.IsFailure ? Problems.From(appealed.Error) : TypedResults.NoContent();
    }

    private static async Task<Ok<MyModerationResponse>> MineAsync(ClaimsPrincipal principal, IModerationQueries queries, CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.IdOf(principal);
        return TypedResults.Ok(new MyModerationResponse(
            await queries.ReportsByAsync(accountId, cancellationToken),
            await queries.ReportsAboutAsync(accountId, cancellationToken)));
    }
}

/// <param name="Id">A UUID the client made for this report; the idempotency key.</param>
/// <param name="Target">Memory or Album.</param>
/// <param name="Reason">ChildAbuse, Terrorism, Threat, Incitement, OtherIllegal (hidden at once, decided within 24 h); SomeonesImage, PersonalData, Defamation, Copyright, Other (decided within 7 days).</param>
/// <param name="Explanation">Why it is wrong, up to 4000 characters.</param>
internal sealed record ReportRequest(Guid Id, string? Target, Guid TargetId, string? Reason, string? Explanation);

/// <param name="Id">The report: this one, or the one about the same thing that already waits.</param>
internal sealed record ReportedResponse(Guid Id);

/// <param name="Text">Why the decision is wrong, up to 4000 characters.</param>
internal sealed record AppealRequest(string? Text);

/// <param name="Reports">What the person reported.</param>
/// <param name="AboutMine">Reports and decisions about the person's own content; who reported is never shown.</param>
internal sealed record MyModerationResponse(IReadOnlyList<MyReport> Reports, IReadOnlyList<ReportAboutMine> AboutMine);

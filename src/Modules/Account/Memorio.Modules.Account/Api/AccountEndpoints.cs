using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Account.Application.Commands.AcceptTerms;
using Memorio.Modules.Account.Application.Queries.AccountDeletionImpact;
using Memorio.Modules.Account.Domain.Accounts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Account.Api;

/// <summary>What a signed-in person does with their own account.</summary>
internal static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder v1)
    {
        var account = v1.MapGroup("/account").WithTags("Account");

        account.MapPost("/terms-acceptance", AcceptTermsAsync)
            .WithName("AcceptTerms")
            .WithSummary("Records that the signed-in person accepted the current version of the terms.");
        account.MapGet("/deletion-impact", GetDeletionImpactAsync)
            .WithName("GetAccountDeletionImpact")
            .WithSummary("In numbers, what goes when the signed-in person deletes their account.");
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> AcceptTermsAsync(
        AcceptTermsRequest request,
        ClaimsPrincipal principal,
        AcceptTermsHandler handler)
    {
        var accepted = await handler.HandleAsync(new AcceptTerms(AccountOf(principal), request.Version));
        return accepted.IsFailure ? Problems.From(accepted.Error) : TypedResults.NoContent();
    }

    private static async Task<Ok<AccountDeletionImpactResponse>> GetDeletionImpactAsync(
        ClaimsPrincipal principal,
        IAccountDeletionImpactQuery query,
        CancellationToken cancellationToken)
    {
        var impact = await query.GetAsync(AccountOf(principal), cancellationToken);
        return TypedResults.Ok(new AccountDeletionImpactResponse(
            impact.Memories,
            impact.Photos,
            impact.OwnAlbums,
            impact.OwnAlbumsWithoutSuccessor,
            impact.OtherPeoplesAlbums));
    }

    /// <summary>The account of the signed-in person; every endpoint here requires sign-in (SD10).</summary>
    private static AccountId AccountOf(ClaimsPrincipal principal) =>
        new(Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!));
}

/// <param name="Version">The version of the terms the person was shown.</param>
internal sealed record AcceptTermsRequest(string? Version);

/// <param name="Memories">Memories the person is the author of.</param>
/// <param name="Photos">Photos in those memories.</param>
/// <param name="OwnAlbums">Albums the person owns.</param>
/// <param name="OwnAlbumsWithoutSuccessor">Of those, albums with no successor named.</param>
/// <param name="OtherPeoplesAlbums">Albums of other owners the person takes part in.</param>
internal sealed record AccountDeletionImpactResponse(
    int Memories,
    int Photos,
    int OwnAlbums,
    int OwnAlbumsWithoutSuccessor,
    int OtherPeoplesAlbums);

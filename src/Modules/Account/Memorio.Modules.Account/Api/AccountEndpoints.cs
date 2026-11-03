using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Account.Application.Commands.AcceptTerms;
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
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> AcceptTermsAsync(
        AcceptTermsRequest request,
        ClaimsPrincipal principal,
        AcceptTermsHandler handler)
    {
        var accepted = await handler.HandleAsync(new AcceptTerms(AccountOf(principal), request.Version));
        return accepted.IsFailure ? Problems.From(accepted.Error) : TypedResults.NoContent();
    }

    /// <summary>The account of the signed-in person; every endpoint here requires sign-in (SD10).</summary>
    private static AccountId AccountOf(ClaimsPrincipal principal) =>
        new(Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!));
}

/// <param name="Version">The version of the terms the person was shown.</param>
internal sealed record AcceptTermsRequest(string? Version);

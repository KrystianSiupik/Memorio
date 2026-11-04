using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Succession.Application.Commands.NameSuccessor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Succession.Api;

internal static class SuccessionEndpoints
{
    public static void MapSuccessionEndpoints(this IEndpointRouteBuilder v1) =>
        v1.MapPut("/albums/{id:guid}/successor", NameAsync)
            .WithTags("Albums")
            .WithName("NameSuccessor")
            .WithSummary("The owner names someone in the album as its successor.");

    private static async Task<Results<NoContent, ProblemHttpResult>> NameAsync(
        Guid id,
        NameSuccessorRequest request,
        ClaimsPrincipal principal,
        NameSuccessorHandler handler,
        CancellationToken cancellationToken)
    {
        var named = await handler.HandleAsync(new NameSuccessor(id, CurrentAccount.IdOf(principal), request.AccountId), cancellationToken);
        return named.IsFailure ? Problems.From(named.Error) : TypedResults.NoContent();
    }
}

internal sealed record NameSuccessorRequest(Guid AccountId);

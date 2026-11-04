using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Notifications.Application.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Notifications.Api;

internal static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this IEndpointRouteBuilder v1) =>
        v1.MapPost("/invitation-block", BlockAsync)
            .WithTags("Notifications")
            .WithName("BlockInvitations")
            .WithSummary("The person's own address takes no more invitations.");

    private static async Task<Results<NoContent, ProblemHttpResult>> BlockAsync(ClaimsPrincipal principal, BlockInvitationsHandler handler, CancellationToken cancellationToken)
    {
        var blocked = await handler.HandleAsync(CurrentAccount.IdOf(principal), cancellationToken);
        return blocked.IsFailure ? Problems.From(blocked.Error) : TypedResults.NoContent();
    }
}

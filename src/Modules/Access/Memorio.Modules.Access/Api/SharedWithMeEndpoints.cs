using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Access.Api;

/// <summary>R14.</summary>
internal static class SharedWithMeEndpoints
{
    public static void MapSharedWithMeEndpoints(this IEndpointRouteBuilder v1) =>
        v1.MapGet("/shared-with-me", GetAsync)
            .WithTags("Access")
            .WithName("GetSharedWithMe")
            .WithSummary("Albums the person may read without being in them. Empty until links exist (E4).");

    private static async Task<Ok<IReadOnlyList<SharedAlbumResponse>>> GetAsync(
        ClaimsPrincipal principal,
        IAccessReader access,
        IAlbumMembership membership,
        IAlbumNames names,
        CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.IdOf(principal);
        var visible = await access.VisibleAlbumsAsync(accountId, cancellationToken);
        var mine = await membership.AlbumsOfAsync(accountId, cancellationToken);
        var shared = visible.Except(mine).ToList();
        var known = await names.NamesOfAsync(shared, cancellationToken);
        return TypedResults.Ok<IReadOnlyList<SharedAlbumResponse>>(
            [.. shared.Where(known.ContainsKey).Select(id => new SharedAlbumResponse(id, known[id]))]);
    }
}

internal sealed record SharedAlbumResponse(Guid Id, string Name);

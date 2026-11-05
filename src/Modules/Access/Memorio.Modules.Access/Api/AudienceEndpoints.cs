using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Access.Application.Commands;
using Memorio.Modules.Access.Application.Queries;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Access.Domain.Audiences;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Access.Api;

/// <summary>Reading an album without being in it: the link, the list of who came in, and blocks.</summary>
internal static class AudienceEndpoints
{
    public static void MapAudienceEndpoints(this IEndpointRouteBuilder v1)
    {
        var albums = v1.MapGroup("/albums/{id:guid}").WithTags("Access");
        albums.MapPost("/link", ShareAsync).WithName("ShareAlbumByLink").WithSummary("The owner gets the link; a working link stays the same.");
        albums.MapGet("/link", GetLinkAsync).WithName("GetAlbumLink").WithSummary("For the owner: the working link, if there is one.");
        albums.MapDelete("/link", RevokeLinkAsync).WithName("RevokeAlbumLink").WithSummary("Nobody new comes in; those let in keep their access.");
        albums.MapGet("/viewers", GetViewersAsync).WithName("GetAlbumViewers").WithSummary("For people in the album: who reads it through the link, by name.");
        albums.MapDelete("/viewers/{accountId:guid}", RevokeAccessAsync).WithName("RevokeAlbumAccess").WithSummary("The owner takes the access of someone let in; they may come back through a working link.");
        albums.MapPut("/blocks/{accountId:guid}", BlockAsync).WithName("BlockUser").WithSummary("The person may not come in by a link or an invitation, and loses access at once.");

        var links = v1.MapGroup("/album-links/{token}").WithTags("Access");
        links.MapGet("/", PreviewAsync).WithName("PreviewAlbumLink").WithSummary("Which album a working link leads to.");
        links.MapPost("/entry", EnterAsync).WithName("EnterAlbumByLink").WithSummary("Comes in through the link, by name.");
    }

    private static async Task<Results<Ok<AlbumLinkResponse>, ProblemHttpResult>> ShareAsync(Guid id, ClaimsPrincipal principal, ShareByLinkHandler handler, CancellationToken cancellationToken)
    {
        var shared = await handler.HandleAsync(id, CurrentAccount.IdOf(principal), cancellationToken);
        return shared.IsFailure ? Problems.From(shared.Error) : TypedResults.Ok(new AlbumLinkResponse(shared.Value));
    }

    private static async Task<Results<Ok<AlbumLinkResponse>, ProblemHttpResult>> GetLinkAsync(Guid id, ClaimsPrincipal principal, IAlbumMembership albums, IAudienceQueries queries, CancellationToken cancellationToken)
    {
        if (await albums.OwnerOfAsync(id, cancellationToken) != CurrentAccount.IdOf(principal))
        {
            return Problems.From(AccessErrors.NoAccess);
        }

        return TypedResults.Ok(new AlbumLinkResponse(await queries.LinkTokenAsync(id, cancellationToken)));
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RevokeLinkAsync(Guid id, ClaimsPrincipal principal, RevokeLinkHandler handler, CancellationToken cancellationToken)
    {
        var revoked = await handler.HandleAsync(id, CurrentAccount.IdOf(principal), cancellationToken);
        return revoked.IsFailure ? Problems.From(revoked.Error) : TypedResults.NoContent();
    }

    private static async Task<Results<Ok<IReadOnlyList<AlbumViewerResponse>>, ProblemHttpResult>> GetViewersAsync(
        Guid id,
        ClaimsPrincipal principal,
        IAlbumMembership albums,
        IAccessReader access,
        IAudienceQueries queries,
        IAccountDirectory directory,
        CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.IdOf(principal);
        if (!await albums.IsMemberAsync(accountId, id, cancellationToken) || !await access.CanSeeAlbumAsync(accountId, id, cancellationToken))
        {
            return Problems.From(AccessErrors.NoAccess);
        }

        var viewers = await queries.ViewersAsync(id, cancellationToken);
        var emails = await directory.EmailsOfAsync([.. viewers.Select(viewer => viewer.AccountId)], cancellationToken);
        return TypedResults.Ok<IReadOnlyList<AlbumViewerResponse>>(
            [.. viewers.Select(viewer => new AlbumViewerResponse(viewer.AccountId, emails.GetValueOrDefault(viewer.AccountId), viewer.EnteredAt))]);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RevokeAccessAsync(Guid id, Guid accountId, ClaimsPrincipal principal, RevokeAccessHandler handler, CancellationToken cancellationToken)
    {
        var revoked = await handler.HandleAsync(id, CurrentAccount.IdOf(principal), accountId, cancellationToken);
        return revoked.IsFailure ? Problems.From(revoked.Error) : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> BlockAsync(Guid id, Guid accountId, ClaimsPrincipal principal, BlockUserHandler handler, CancellationToken cancellationToken)
    {
        var blocked = await handler.HandleAsync(id, CurrentAccount.IdOf(principal), accountId, cancellationToken);
        return blocked.IsFailure ? Problems.From(blocked.Error) : TypedResults.NoContent();
    }

    private static async Task<Results<Ok<AlbumLinkPreviewResponse>, ProblemHttpResult>> PreviewAsync(
        string token,
        ClaimsPrincipal principal,
        IAudienceQueries queries,
        IBlockList blocks,
        IAlbumNames names,
        CancellationToken cancellationToken)
    {
        if (await queries.AlbumOfTokenAsync(token, cancellationToken) is not { } albumId)
        {
            return Problems.From(AccessErrors.LinkRevoked);
        }

        if (await blocks.IsBlockedAsync(albumId, CurrentAccount.IdOf(principal), cancellationToken))
        {
            return Problems.From(AccessErrors.Blocked);
        }

        var known = await names.NamesOfAsync([albumId], cancellationToken);
        return known.TryGetValue(albumId, out var name)
            ? TypedResults.Ok(new AlbumLinkPreviewResponse(albumId, name))
            : Problems.From(AccessErrors.LinkRevoked);
    }

    private static async Task<Results<Ok<EnteredAlbumResponse>, ProblemHttpResult>> EnterAsync(string token, ClaimsPrincipal principal, EnterByLinkHandler handler, CancellationToken cancellationToken)
    {
        var entered = await handler.HandleAsync(token, CurrentAccount.IdOf(principal), cancellationToken);
        return entered.IsFailure ? Problems.From(entered.Error) : TypedResults.Ok(new EnteredAlbumResponse(entered.Value));
    }
}

/// <param name="Token">What the link carries; null when there is no working link.</param>
internal sealed record AlbumLinkResponse(string? Token);

internal sealed record AlbumViewerResponse(Guid AccountId, string? Email, DateTimeOffset EnteredAt);

internal sealed record AlbumLinkPreviewResponse(Guid AlbumId, string AlbumName);

internal sealed record EnteredAlbumResponse(Guid AlbumId);

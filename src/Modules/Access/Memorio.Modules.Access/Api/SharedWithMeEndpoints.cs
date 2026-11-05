using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Access.Api;

/// <summary>R14.</summary>
internal static class SharedWithMeEndpoints
{
    public static void MapSharedWithMeEndpoints(this IEndpointRouteBuilder v1)
    {
        v1.MapGet("/shared-with-me", GetAsync)
            .WithTags("Access")
            .WithName("GetSharedWithMe")
            .WithSummary("Albums the person may read without being in them.");
        v1.MapGet("/legacy-archives", GetLegacyAsync)
            .WithTags("Access")
            .WithName("ListLegacyArchives")
            .WithSummary("People who named this person as their legacy contact and have died: their private memories can be read.");
    }

    private static async Task<Ok<IReadOnlyList<LegacyArchiveResponse>>> GetLegacyAsync(
        ClaimsPrincipal principal,
        IAccessReader access,
        IAccountDirectory directory,
        CancellationToken cancellationToken)
    {
        var owners = await access.LegacyArchivesAsync(CurrentAccount.IdOf(principal), cancellationToken);
        var emails = await directory.EmailsOfAsync(owners, cancellationToken);
        return TypedResults.Ok<IReadOnlyList<LegacyArchiveResponse>>([.. owners.Select(owner => new LegacyArchiveResponse(owner, emails.GetValueOrDefault(owner)))]);
    }

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

internal sealed record LegacyArchiveResponse(Guid OwnerAccountId, string? Email);

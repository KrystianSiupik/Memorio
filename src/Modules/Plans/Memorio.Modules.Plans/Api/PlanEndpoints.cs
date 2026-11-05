using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Plans.Application.Commands.Cycle;
using Memorio.Modules.Plans.Application.Queries;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Plans.Api;

/// <summary>A plan as its payer sees it: the person's own, or the plan of an album they own. Nobody else sees a plan.</summary>
internal static class PlanEndpoints
{
    public static void MapPlanEndpoints(this IEndpointRouteBuilder v1)
    {
        var own = v1.MapGroup("/plan").WithTags("Plans");
        own.MapGet("/", GetOwnAsync).WithName("GetPlan").WithSummary("The plan of the person's account: mode, dates and what is used.");
        own.MapPost("/refresh", RefreshOwnAsync).WithName("RefreshPlan").WithSummary("Asks the payment source now instead of waiting for its notification.");
        own.MapPut("/kept-material", KeepOwnAsync).WithName("KeepMaterial").WithSummary("In the window before the trim: the memories to keep first; a new choice replaces the earlier one.");

        var album = v1.MapGroup("/albums/{id:guid}/plan").WithTags("Plans");
        album.MapGet("/", GetAlbumAsync).WithName("GetAlbumPlan").WithSummary("For the owner: the plan of the album.");
        album.MapPost("/refresh", RefreshAlbumAsync).WithName("RefreshAlbumPlan").WithSummary("For the owner: asks the payment source now.");
        album.MapPut("/kept-material", KeepAlbumAsync).WithName("KeepAlbumMaterial").WithSummary("For the owner, in the window before the trim: the memories to keep first.");
    }

    private static Task<Ok<PlanResponse>> GetOwnAsync(ClaimsPrincipal principal, IPlanStateQuery plans, IPoolState pools, TimeProvider clock, CancellationToken cancellationToken) =>
        ReadAsync(new EntitledResource(EntitledResourceKind.Account, CurrentAccount.IdOf(principal)), plans, pools, clock, cancellationToken);

    private static async Task<Results<Ok<PlanResponse>, ProblemHttpResult>> RefreshOwnAsync(ClaimsPrincipal principal, RefreshSubscriptionHandler refresh, IPlanStateQuery plans, IPoolState pools, TimeProvider clock, CancellationToken cancellationToken)
    {
        var resource = new EntitledResource(EntitledResourceKind.Account, CurrentAccount.IdOf(principal));
        var refreshed = await refresh.HandleAsync(resource, cancellationToken);
        return refreshed.IsFailure ? Problems.From(refreshed.Error) : await ReadAsync(resource, plans, pools, clock, cancellationToken);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> KeepOwnAsync(KeepMaterialRequest request, ClaimsPrincipal principal, KeepMaterialHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new KeepMaterial(new EntitledResource(EntitledResourceKind.Account, CurrentAccount.IdOf(principal)), request.MemoryIds ?? []), cancellationToken));

    private static async Task<Results<Ok<PlanResponse>, ProblemHttpResult>> GetAlbumAsync(Guid id, ClaimsPrincipal principal, IAlbumMembership albums, IPlanStateQuery plans, IPoolState pools, TimeProvider clock, CancellationToken cancellationToken) =>
        !await OwnsAsync(id, principal, albums, cancellationToken)
            ? Problems.From(PlanCycleErrors.NoAccess)
            : await ReadAsync(new EntitledResource(EntitledResourceKind.Album, id), plans, pools, clock, cancellationToken);

    private static async Task<Results<Ok<PlanResponse>, ProblemHttpResult>> RefreshAlbumAsync(Guid id, ClaimsPrincipal principal, IAlbumMembership albums, RefreshSubscriptionHandler refresh, IPlanStateQuery plans, IPoolState pools, TimeProvider clock, CancellationToken cancellationToken)
    {
        if (!await OwnsAsync(id, principal, albums, cancellationToken))
        {
            return Problems.From(PlanCycleErrors.NoAccess);
        }

        var resource = new EntitledResource(EntitledResourceKind.Album, id);
        var refreshed = await refresh.HandleAsync(resource, cancellationToken);
        return refreshed.IsFailure ? Problems.From(refreshed.Error) : await ReadAsync(resource, plans, pools, clock, cancellationToken);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> KeepAlbumAsync(Guid id, KeepMaterialRequest request, ClaimsPrincipal principal, IAlbumMembership albums, KeepMaterialHandler handler, CancellationToken cancellationToken) =>
        !await OwnsAsync(id, principal, albums, cancellationToken)
            ? Problems.From(PlanCycleErrors.NoAccess)
            : Answer(await handler.HandleAsync(new KeepMaterial(new EntitledResource(EntitledResourceKind.Album, id), request.MemoryIds ?? []), cancellationToken));

    private static async Task<bool> OwnsAsync(Guid albumId, ClaimsPrincipal principal, IAlbumMembership albums, CancellationToken cancellationToken) =>
        await albums.OwnerOfAsync(albumId, cancellationToken) == CurrentAccount.IdOf(principal);

    private static async Task<Ok<PlanResponse>> ReadAsync(EntitledResource resource, IPlanStateQuery plans, IPoolState pools, TimeProvider clock, CancellationToken cancellationToken)
    {
        var plan = await plans.GetAsync(resource, cancellationToken);
        var pool = await pools.GetAsync(resource.Id, cancellationToken);
        var now = clock.GetUtcNow();
        return TypedResults.Ok(new PlanResponse(
            plan.Mode,
            plan.EntitledUntil,
            plan.ReadOnlySince,
            plan.TrimAt,
            plan.ChoiceOpensAt,
            plan.Mode == nameof(PlanMode.ReadOnly) && now >= plan.ChoiceOpensAt && (now < plan.TrimAt || plan.IsHeld),
            plan.IsHeld,
            plan.LastCheckedAt,
            plan.KeptMemoryIds,
            pool.UsedBytes,
            pool.LimitBytes,
            PlanTerms.Free.LimitBytes,
            pool.AiUsed,
            pool.AiLimit));
    }

    private static Results<NoContent, ProblemHttpResult> Answer(Result result) =>
        result.IsSuccess ? TypedResults.NoContent() : Problems.From(result.Error);
}

/// <param name="Mode">Free, Active, ReadOnly or Trimmed.</param>
/// <param name="TrimAt">When what is over the free limit goes to the trash, while read-only.</param>
/// <param name="ChoiceOpensAt">When the window to choose what stays opens.</param>
/// <param name="IsHeld">A warning did not reach someone: the trim waits (S2:C10).</param>
internal sealed record PlanResponse(
    string Mode,
    DateTimeOffset? EntitledUntil,
    DateTimeOffset? ReadOnlySince,
    DateTimeOffset? TrimAt,
    DateTimeOffset? ChoiceOpensAt,
    bool IsChoiceOpen,
    bool IsHeld,
    DateTimeOffset? LastCheckedAt,
    IReadOnlyList<Guid> KeptMemoryIds,
    long UsedBytes,
    long LimitBytes,
    long FreeLimitBytes,
    int AiUsed,
    int AiLimit);

internal sealed record KeepMaterialRequest(IReadOnlyList<Guid>? MemoryIds);

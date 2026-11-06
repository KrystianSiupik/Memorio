using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Application.Commands.CreateAlbum;
using Memorio.Modules.Album.Application.Commands.Invitations;
using Memorio.Modules.Album.Application.Commands.Takeover;
using Memorio.Modules.Album.Application.Queries;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Album.Api;

/// <summary>Albums people build together: who is in them and what each of them brought.</summary>
internal static class AlbumsEndpoints
{
    public static void MapAlbumsEndpoints(this IEndpointRouteBuilder v1)
    {
        var albums = v1.MapGroup("/albums").WithTags("Albums");

        albums.MapPost("/", CreateAsync).WithName("CreateAlbum").WithSummary("Creates an album; the id comes from the client, so a retry changes nothing.");
        albums.MapPost("/from-period", CreateFromPeriodAsync).WithName("CreateAlbumFromPeriod").WithSummary("An album of the person's own memories of a period (yyyy-MM-dd); the id comes from the client, so a retry changes nothing.");
        albums.MapGet("/", ListAsync).WithName("ListAlbums").WithSummary("The albums the person owns or takes part in.");
        albums.MapGet("/kept-contributions", KeptAsync).WithName("ListKeptContributions").WithSummary("What stayed in albums the person is no longer in.");
        albums.MapGet("/entrusted", EntrustedAsync).WithName("ListEntrusted").WithSummary("Memories entrusted to the person as a curator.");
        albums.MapGet("/{id:guid}", GetAsync).WithName("GetAlbum").WithSummary("The album with its memories, members, the person's own contributions and the pool.");
        albums.MapGet("/{id:guid}/trim-material", TrimMaterialAsync).WithName("GetAlbumTrimMaterial").WithSummary("For the owner: what a trim of the album could take, oldest first, with sizes.");
        albums.MapGet("/{id:guid}/takeover", TakeoverAsync).WithName("GetAlbumTakeover").WithSummary("For someone in an album without an owner: whether it can be taken over, and who holds it until when.");
        albums.MapPost("/{id:guid}/takeover-reservation", ReserveTakeoverAsync).WithName("ReserveAlbumTakeover").WithSummary("Holds an album without an owner for 30 minutes, to pay for its plan; asking again keeps one's own.");
        albums.MapGet("/{id:guid}/seats", SeatsAsync).WithName("GetAlbumSeats").WithSummary("For the owner: seats taken, invited and free.");
        albums.MapPost("/{id:guid}/invitations", InviteAsync).WithName("InviteToAlbum").WithSummary("Invites an address; the same address again refreshes its invitation and takes no second seat.");
        albums.MapDelete("/{id:guid}/invitations/{invitationId:guid}", WithdrawInvitationAsync).WithName("WithdrawInvitation").WithSummary("The owner takes an invitation back; the seat is free at once.");
        albums.MapDelete("/{id:guid}/participants/{accountId:guid}", RemoveParticipantAsync).WithName("RemoveParticipant").WithSummary("The owner removes a person; what they brought stays until they choose.");
        albums.MapPost("/{id:guid}/leave", LeaveAsync).WithName("LeaveAlbum").WithSummary("Leaves the album, keeping what the person brought in it or taking it along.");
        albums.MapPost("/{id:guid}/memories", AddMemoryAsync).WithName("AddMemoryToAlbum").WithSummary("The author brings their memory; the album's pool pays for it.");
        albums.MapPost("/{id:guid}/memories/{memoryId:guid}/photos", AddPhotoAsync).WithName("AddPhotoInAlbum").WithSummary("The author adds an uploaded photo to their memory in the album; the album's pool pays for it.");
        albums.MapPut("/{id:guid}/memories/{memoryId:guid}/tags/{accountId:guid}", TagAsync).WithName("TagParticipant").WithSummary("Tags someone in this album in the memory.");
        albums.MapDelete("/{id:guid}/memories/{memoryId:guid}/tags/me", UntagAsync).WithName("RemoveOwnTag").WithSummary("Removes the person's own tag.");
        albums.MapPost("/{id:guid}/contributions/withdrawal", RevokeKeptAsync).WithName("RevokeKeptContributions").WithSummary("Takes back what stayed in the album after the person left or was removed.");
        albums.MapPut("/{id:guid}/curator", EntrustAsync).WithName("EntrustToCurator").WithSummary("Lets someone in the album put the person's contributions in order.");
        albums.MapDelete("/{id:guid}/curator", RevokeEntrustmentAsync).WithName("RevokeEntrustment").WithSummary("Ends the entrustment; asking again changes nothing.");
        albums.MapPut("/{id:guid}/arrangement", ArrangeAsync).WithName("ArrangeEntrusted").WithSummary("The curator orders the memories entrusted to them.");

        var invitations = v1.MapGroup("/invitations").WithTags("Albums");
        invitations.MapGet("/", ListInvitationsAsync).WithName("ListInvitations").WithSummary("Invitations waiting for the person's address: who invites and to what, no content.");
        invitations.MapPost("/{id:guid}/acceptance", AcceptAsync).WithName("AcceptInvitation").WithSummary("Joins the album.");
        invitations.MapPost("/{id:guid}/refusal", DeclineAsync).WithName("DeclineInvitation").WithSummary("Refuses; the seat is free at once.");
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> CreateAsync(CreateAlbumRequest request, ClaimsPrincipal principal, CreateAlbumHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new CreateAlbum(request.Id, CurrentAccount.IdOf(principal), request.Name), cancellationToken));

    private static async Task<Results<Ok<PeriodAlbumResponse>, ProblemHttpResult>> CreateFromPeriodAsync(CreateAlbumFromPeriodRequest request, ClaimsPrincipal principal, CreateAlbumFromPeriodHandler handler, CancellationToken cancellationToken)
    {
        var culture = System.Globalization.CultureInfo.InvariantCulture;
        if (!DateOnly.TryParseExact(request.From, "yyyy-MM-dd", culture, System.Globalization.DateTimeStyles.None, out var from)
            || !DateOnly.TryParseExact(request.To, "yyyy-MM-dd", culture, System.Globalization.DateTimeStyles.None, out var to)
            || from > to)
        {
            return Problems.From(AlbumErrors.InvalidPeriod);
        }

        var created = await handler.HandleAsync(new CreateAlbumFromPeriod(request.Id, CurrentAccount.IdOf(principal), request.Name, from, to), cancellationToken);
        return created.IsFailure ? Problems.From(created.Error) : TypedResults.Ok(new PeriodAlbumResponse(created.Value.Added, created.Value.NotFitting));
    }

    private static async Task<Ok<IReadOnlyList<AlbumListItemResponse>>> ListAsync(ClaimsPrincipal principal, IAlbumQueries queries, CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.IdOf(principal);
        var albums = await queries.MyAlbumsAsync(accountId, cancellationToken);
        return TypedResults.Ok<IReadOnlyList<AlbumListItemResponse>>(
            [.. albums.Select(album => new AlbumListItemResponse(album.Id, album.Name, album.OwnerAccountId == accountId, album.Members, album.Memories))]);
    }

    private static async Task<Results<Ok<AlbumResponse>, ProblemHttpResult>> GetAsync(
        Guid id,
        ClaimsPrincipal principal,
        IAccessReader access,
        IAlbumQueries queries,
        IAlbumContent content,
        IAlbumPool pool,
        IAccountDirectory directory,
        IAlbumSeatLimit plan,
        CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.IdOf(principal);
        if (!await access.CanSeeAlbumAsync(accountId, id, cancellationToken) || await queries.ViewAsync(id, cancellationToken) is not { } album)
        {
            return Problems.From(AlbumErrors.NoAccess);
        }

        var summaries = await content.SummarizeAsync([.. album.Memories.Select(memory => memory.MemoryId)], cancellationToken);
        var emails = await directory.EmailsOfAsync(
            [.. album.Members.Select(member => member.AccountId).Union(album.Memories.Select(memory => memory.AuthorAccountId))],
            cancellationToken);
        var state = await pool.GetStateAsync(id, cancellationToken);
        var visible = album.Memories.Where(memory => summaries.ContainsKey(memory.MemoryId)).ToList();
        var entrustment = await queries.MyEntrustmentAsync(id, accountId, cancellationToken);

        // Someone reading through a link sees the content, not who is in the album or their addresses (D-271).
        var isMember = album.Members.Any(member => member.AccountId == accountId);
        return TypedResults.Ok(new AlbumResponse(
            album.Id,
            album.Name,
            album.OwnerAccountId == accountId,
            isMember,
            isMember ? album.SuccessorAccountId : null,
            isMember ? [.. album.Members.Select(member => new AlbumMemberResponse(member.AccountId, emails.GetValueOrDefault(member.AccountId), member.IsOwner))] : [],
            [.. visible.Select(memory => new AlbumMemoryResponse(
                memory.MemoryId,
                isMember ? memory.AuthorAccountId : null,
                isMember ? emails.GetValueOrDefault(memory.AuthorAccountId) : null,
                memory.AuthorAccountId == accountId,
                summaries[memory.MemoryId].PlaceName,
                summaries[memory.MemoryId].DateFrom,
                summaries[memory.MemoryId].Photos,
                isMember ? memory.TaggedAccountIds : []))],
            visible.Count(memory => memory.AuthorAccountId == accountId),
            visible.Where(memory => memory.AuthorAccountId == accountId).Sum(memory => summaries[memory.MemoryId].Photos),
            state.FreeBytes,
            state.LimitBytes,
            entrustment?.CuratorAccountId,
            await plan.IsReadOnlyAsync(id, cancellationToken),
            album.OwnerAccountId == SharedAlbum.NoOwner));
    }

    private static async Task<Results<Ok<AlbumSeatsResponse>, ProblemHttpResult>> SeatsAsync(Guid id, ClaimsPrincipal principal, IAlbumQueries queries, IAlbumSeatLimit limit, CancellationToken cancellationToken)
    {
        if (await queries.SeatsAsync(id, CurrentAccount.IdOf(principal), cancellationToken) is not { } seats)
        {
            return Problems.From(AlbumErrors.NoAccess);
        }

        var taken = 1 + seats.Participants;
        var total = await limit.SeatsOfAsync(id, cancellationToken);
        return TypedResults.Ok(new AlbumSeatsResponse(
            taken,
            seats.Invited.Count,
            Math.Max(0, total - taken - seats.Invited.Count),
            [.. seats.Invited.Select(invited => new AlbumInvitedResponse(invited.Id, invited.Email))]));
    }

    private static async Task<Results<Ok<IReadOnlyList<AlbumTrimMaterialResponse>>, ProblemHttpResult>> TrimMaterialAsync(Guid id, ClaimsPrincipal principal, IAlbumMembership membership, IAlbumContent content, CancellationToken cancellationToken)
    {
        if (await membership.OwnerOfAsync(id, cancellationToken) != CurrentAccount.IdOf(principal))
        {
            return Problems.From(AlbumErrors.NoAccess);
        }

        var items = await content.TrimMaterialAsync(id, cancellationToken);
        return TypedResults.Ok<IReadOnlyList<AlbumTrimMaterialResponse>>(
            [.. items.Select(item => new AlbumTrimMaterialResponse(item.MemoryId, item.PlaceName, item.DateFrom, item.SizeBytes))]);
    }

    private static async Task<Results<Ok<AlbumTakeoverResponse>, ProblemHttpResult>> TakeoverAsync(Guid id, ClaimsPrincipal principal, IAlbumMembership membership, IAlbumQueries queries, TimeProvider clock, CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.IdOf(principal);
        if (!await membership.IsMemberAsync(accountId, id, cancellationToken) || await queries.TakeoverStateAsync(id, cancellationToken) is not { } state)
        {
            return Problems.From(AlbumErrors.NoAccess);
        }

        var reserved = state.ReservedUntil > clock.GetUtcNow();
        return TypedResults.Ok(new AlbumTakeoverResponse(
            state.IsOwnerless,
            state.IsOwnerless && !reserved,
            reserved && state.ReservedByAccountId == accountId,
            reserved ? state.ReservedUntil : null));
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> ReserveTakeoverAsync(Guid id, ClaimsPrincipal principal, ReserveTakeoverHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new ReserveTakeover(new AlbumId(id), CurrentAccount.IdOf(principal)), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> InviteAsync(Guid id, InviteRequest request, ClaimsPrincipal principal, InviteToAlbumHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new InviteToAlbum(new AlbumId(id), CurrentAccount.IdOf(principal), request.Id, request.Email), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> WithdrawInvitationAsync(Guid id, Guid invitationId, ClaimsPrincipal principal, WithdrawInvitationHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new WithdrawInvitation(new AlbumId(id), CurrentAccount.IdOf(principal), invitationId), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> RemoveParticipantAsync(Guid id, Guid accountId, ClaimsPrincipal principal, RemoveParticipantHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new RemoveParticipant(new AlbumId(id), CurrentAccount.IdOf(principal), accountId), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> LeaveAsync(Guid id, LeaveRequest request, ClaimsPrincipal principal, LeaveAlbumHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new LeaveAlbum(new AlbumId(id), CurrentAccount.IdOf(principal), request.KeepContributions), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> AddMemoryAsync(Guid id, AddMemoryRequest request, ClaimsPrincipal principal, AddMemoryToAlbumHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new AddMemoryToAlbum(new AlbumId(id), CurrentAccount.IdOf(principal), request.MemoryId), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> AddPhotoAsync(Guid id, Guid memoryId, AddAlbumPhotoRequest request, ClaimsPrincipal principal, AddPhotoInAlbumHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new AddPhotoInAlbum(new AlbumId(id), CurrentAccount.IdOf(principal), memoryId, request.PhotoId, request.UploadKey), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> TagAsync(Guid id, Guid memoryId, Guid accountId, ClaimsPrincipal principal, TagParticipantHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new TagParticipant(new AlbumId(id), CurrentAccount.IdOf(principal), memoryId, accountId), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> UntagAsync(Guid id, Guid memoryId, ClaimsPrincipal principal, RemoveOwnTagHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new RemoveOwnTag(new AlbumId(id), CurrentAccount.IdOf(principal), memoryId), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> RevokeKeptAsync(Guid id, ClaimsPrincipal principal, RevokeKeptContributionsHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new RevokeKeptContributions(new AlbumId(id), CurrentAccount.IdOf(principal)), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> EntrustAsync(Guid id, EntrustRequest request, ClaimsPrincipal principal, EntrustToCuratorHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new EntrustToCurator(new AlbumId(id), CurrentAccount.IdOf(principal), request.CuratorAccountId), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> RevokeEntrustmentAsync(Guid id, ClaimsPrincipal principal, RevokeEntrustmentHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new RevokeEntrustment(new AlbumId(id), CurrentAccount.IdOf(principal)), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> ArrangeAsync(Guid id, ArrangeRequest request, ClaimsPrincipal principal, ArrangeEntrustedHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new ArrangeEntrusted(new AlbumId(id), CurrentAccount.IdOf(principal), request.MemoryIds ?? []), cancellationToken));

    private static async Task<Ok<IReadOnlyList<KeptContributionResponse>>> KeptAsync(ClaimsPrincipal principal, IAlbumQueries queries, CancellationToken cancellationToken)
    {
        var kept = await queries.KeptContributionsAsync(CurrentAccount.IdOf(principal), cancellationToken);
        return TypedResults.Ok<IReadOnlyList<KeptContributionResponse>>(
            [.. kept.Select(item => new KeptContributionResponse(item.AlbumId, item.AlbumName, item.ChosenByAuthor, item.Memories))]);
    }

    private static async Task<Ok<IReadOnlyList<EntrustedMemoryResponse>>> EntrustedAsync(ClaimsPrincipal principal, IAlbumQueries queries, IAlbumContent content, CancellationToken cancellationToken)
    {
        var entrusted = await queries.EntrustedToAsync(CurrentAccount.IdOf(principal), cancellationToken);
        var summaries = await content.SummarizeAsync([.. entrusted.Select(item => item.MemoryId)], cancellationToken);
        return TypedResults.Ok<IReadOnlyList<EntrustedMemoryResponse>>(
            [.. entrusted.Where(item => summaries.ContainsKey(item.MemoryId)).Select(item => new EntrustedMemoryResponse(
                item.AlbumId, item.AlbumName, item.MemoryId, summaries[item.MemoryId].PlaceName, summaries[item.MemoryId].DateFrom))]);
    }

    private static async Task<Ok<IReadOnlyList<InvitationResponse>>> ListInvitationsAsync(ClaimsPrincipal principal, IAlbumQueries queries, IAccountDirectory directory, CancellationToken cancellationToken)
    {
        var email = InvitationEmail.Normalize(await directory.EmailOfAsync(CurrentAccount.IdOf(principal), cancellationToken));
        if (email is null)
        {
            return TypedResults.Ok<IReadOnlyList<InvitationResponse>>([]);
        }

        var invitations = await queries.PendingInvitationsAsync(email, cancellationToken);
        var inviters = await directory.EmailsOfAsync([.. invitations.Select(item => item.InvitedByAccountId).Distinct()], cancellationToken);
        return TypedResults.Ok<IReadOnlyList<InvitationResponse>>(
            [.. invitations.Select(item => new InvitationResponse(item.Id, item.AlbumId, item.AlbumName, inviters.GetValueOrDefault(item.InvitedByAccountId)))]);
    }

    private static async Task<Results<Ok<AnsweredInvitationResponse>, ProblemHttpResult>> AcceptAsync(Guid id, ClaimsPrincipal principal, AnswerInvitationHandler handler, CancellationToken cancellationToken)
    {
        var answered = await handler.HandleAsync(new AnswerInvitation(id, CurrentAccount.IdOf(principal), Accepted: true), cancellationToken);
        return answered.IsFailure ? Problems.From(answered.Error) : TypedResults.Ok(new AnsweredInvitationResponse(answered.Value.Value));
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DeclineAsync(Guid id, ClaimsPrincipal principal, AnswerInvitationHandler handler, CancellationToken cancellationToken)
    {
        var answered = await handler.HandleAsync(new AnswerInvitation(id, CurrentAccount.IdOf(principal), Accepted: false), cancellationToken);
        return answered.IsFailure ? Problems.From(answered.Error) : TypedResults.NoContent();
    }

    private static Results<NoContent, ProblemHttpResult> Answer(Result result) =>
        result.IsFailure ? Problems.From(result.Error) : TypedResults.NoContent();
}

/// <param name="Id">A UUID the client made for this album; the idempotency key.</param>
internal sealed record CreateAlbumRequest(Guid Id, string? Name);

internal sealed record CreateAlbumFromPeriodRequest(Guid Id, string? Name, string? From, string? To);

internal sealed record PeriodAlbumResponse(int Added, int NotFitting);

/// <param name="Id">A UUID the client made for this invitation.</param>
internal sealed record InviteRequest(Guid Id, string? Email);

/// <param name="KeepContributions">True keeps what the person brought in the album; false takes it along.</param>
internal sealed record LeaveRequest(bool KeepContributions);

internal sealed record AddMemoryRequest(Guid MemoryId);

/// <param name="PhotoId">A UUID the client made for this photo; the idempotency key.</param>
/// <param name="UploadKey">The key from starting the upload.</param>
internal sealed record AddAlbumPhotoRequest(Guid PhotoId, string? UploadKey);

internal sealed record EntrustRequest(Guid CuratorAccountId);

/// <param name="MemoryIds">In the wanted order.</param>
internal sealed record ArrangeRequest(IReadOnlyList<Guid>? MemoryIds);

internal sealed record AlbumListItemResponse(Guid Id, string Name, bool IsOwner, int Members, int Memories);

/// <param name="IsMember">False for someone reading through a link: no people, no addresses, no actions.</param>
/// <param name="MyMemories">How many memories the person brought.</param>
/// <param name="MyPhotos">How many photos are in them.</param>
/// <param name="MyCuratorAccountId">Who the person entrusted their contributions to, if anyone.</param>
internal sealed record AlbumResponse(
    Guid Id,
    string Name,
    bool IsOwner,
    bool IsMember,
    Guid? SuccessorAccountId,
    IReadOnlyList<AlbumMemberResponse> Members,
    IReadOnlyList<AlbumMemoryResponse> Memories,
    int MyMemories,
    int MyPhotos,
    long FreeBytes,
    long LimitBytes,
    Guid? MyCuratorAccountId,
    bool IsReadOnly,
    bool IsOwnerless);

internal sealed record AlbumTrimMaterialResponse(Guid MemoryId, string PlaceName, string DateFrom, long SizeBytes);

internal sealed record AlbumTakeoverResponse(bool IsOwnerless, bool CanReserve, bool ReservedByMe, DateTimeOffset? ReservedUntil);

internal sealed record AlbumMemberResponse(Guid AccountId, string? Email, bool IsOwner);

internal sealed record AlbumMemoryResponse(Guid Id, Guid? AuthorAccountId, string? AuthorEmail, bool IsMine, string PlaceName, string DateFrom, int Photos, IReadOnlyList<Guid> TaggedAccountIds);

internal sealed record AlbumSeatsResponse(int Taken, int Invited, int Free, IReadOnlyList<AlbumInvitedResponse> Invitations);

internal sealed record AlbumInvitedResponse(Guid Id, string Email);

internal sealed record KeptContributionResponse(Guid AlbumId, string AlbumName, bool ChosenByAuthor, int Memories);

internal sealed record EntrustedMemoryResponse(Guid AlbumId, string AlbumName, Guid MemoryId, string PlaceName, string DateFrom);

internal sealed record InvitationResponse(Guid Id, Guid AlbumId, string AlbumName, string? InvitedBy);

internal sealed record AnsweredInvitationResponse(Guid AlbumId);

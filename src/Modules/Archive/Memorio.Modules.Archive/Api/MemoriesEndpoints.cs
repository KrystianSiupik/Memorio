using System.Security.Claims;
using Memorio.Modules.Access.Contracts;
using Memorio.Modularity;
using Memorio.Modules.Archive.Application.Commands.AddPhoto;
using Memorio.Modules.Archive.Application.Commands.RestoreMemory;
using Memorio.Modules.Archive.Application.Commands.DeleteMemory;
using Memorio.Modules.Archive.Application.Commands.TrashMemory;
using Memorio.Modules.Archive.Application.Commands.ApproveDescription;
using Memorio.Modules.Archive.Application.Commands.CreateMemory;
using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Memorio.Modules.Archive.Application.Queries.MemoryMap;
using Memorio.Modules.Archive.Application.Queries.MyMemories;
using Memorio.Modules.Archive.Application.Queries.Trash;
using Memorio.Modules.Archive.Application.Queries.TrimMaterial;
using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Archive.Api;

/// <summary>Memories on the private map.</summary>
internal static class MemoriesEndpoints
{
    public static void MapMemoriesEndpoints(this IEndpointRouteBuilder v1)
    {
        var memories = v1.MapGroup("/memories").WithTags("Memories");

        memories.MapPost("/", CreateAsync)
            .WithName("CreateMemory")
            .WithSummary("Records a memory; the id comes from the client, so a retry changes nothing.");
        memories.MapGet("/map", GetMapAsync)
            .WithName("GetMemoryMap")
            .WithSummary("One point per memory the person may see; from and to (yyyy-MM-dd) filter by overlap.");
        memories.MapGet("/mine", GetMineAsync)
            .WithName("ListMyMemories")
            .WithSummary("The person's own active memories, newest first.");
        memories.MapGet("/legacy/{ownerAccountId:guid}", GetLegacyAsync)
            .WithName("ListLegacyMemories")
            .WithSummary("For the legacy contact of a person who died: their private memories, oldest first.");
        memories.MapGet("/trim-material", GetTrimMaterialAsync)
            .WithName("GetTrimMaterial")
            .WithSummary("What a trim of the person's archive could take, oldest first, with sizes.");
        memories.MapPost("/{id:guid}/restore", RestoreAsync)
            .WithName("RestoreMemory")
            .WithSummary("Takes the person's content back from the trash while its 30 days last.");
        memories.MapGet("/trash", GetTrashAsync)
            .WithName("GetTrash")
            .WithSummary("The person's own content in the trash, while its 30 days last.");
        memories.MapPost("/{id:guid}/permanent-deletion", RequestDataDeletionAsync)
            .WithName("RequestDataDeletion")
            .WithSummary("The author's formal request: the memory goes for good at once, without the trash.");
        memories.MapDelete("/{id:guid}", TrashAsync)
            .WithName("TrashMemory")
            .WithSummary("Moves the person's content to the trash for 30 days.");
        memories.MapGet("/{id:guid}", GetDetailsAsync)
            .WithName("GetMemory")
            .WithSummary("Everything about one memory the person may see.");
        memories.MapPut("/{id:guid}/description", ApproveDescriptionAsync)
            .WithName("ApproveDescription")
            .WithSummary("The author approves the description of a memory, naming the AI draft it started from, if any.");
        memories.MapPost("/uploads", StartUploadAsync)
            .WithName("StartPhotoUpload")
            .WithSummary("An address to send one photo to, valid for 5 minutes.");
        memories.MapPost("/{id:guid}/photos", AddPhotoAsync)
            .WithName("AddPhoto")
            .WithSummary("Adds an uploaded photo to a memory; the photo id comes from the client, so a retry changes nothing.");
    }

    private static async Task<Results<Ok<CreatedMemoryResponse>, ProblemHttpResult>> CreateAsync(
        CreateMemoryRequest request,
        ClaimsPrincipal principal,
        CreateMemoryHandler handler,
        CancellationToken cancellationToken)
    {
        var created = await handler.HandleAsync(
            new CreateMemory(request.Id, CurrentAccount.IdOf(principal), request.PlaceReference, request.DatePrecision, request.DateFrom, request.DateTo),
            cancellationToken);
        return created.IsFailure ? Problems.From(created.Error) : TypedResults.Ok(new CreatedMemoryResponse(created.Value.Id.Value));
    }

    private static async Task<Results<Ok<UploadResponse>, ProblemHttpResult>> StartUploadAsync(
        StartUploadRequest request,
        StartUploadHandler handler,
        CancellationToken cancellationToken)
    {
        var slot = await handler.HandleAsync(request.ContentType, cancellationToken);
        return slot.IsFailure
            ? Problems.From(slot.Error)
            : TypedResults.Ok(new UploadResponse(slot.Value.Key.Value, slot.Value.Upload.Url, slot.Value.ContentType));
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> AddPhotoAsync(
        Guid id,
        AddPhotoRequest request,
        ClaimsPrincipal principal,
        AddPhotoHandler handler,
        CancellationToken cancellationToken)
    {
        var added = await handler.HandleAsync(
            new AddPhoto(new MemoryId(id), CurrentAccount.IdOf(principal), request.PhotoId, request.UploadKey),
            cancellationToken);
        return added.IsFailure ? Problems.From(added.Error) : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> ApproveDescriptionAsync(
        Guid id,
        ApproveDescriptionRequest request,
        ClaimsPrincipal principal,
        ApproveDescriptionHandler handler,
        CancellationToken cancellationToken)
    {
        var approved = await handler.HandleAsync(
            new ApproveDescription(new MemoryId(id), CurrentAccount.IdOf(principal), request.Text, request.AiDraftId),
            cancellationToken);
        return approved.IsFailure ? Problems.From(approved.Error) : TypedResults.NoContent();
    }

    private static async Task<Results<Ok<MemoryDetailsResponse>, ProblemHttpResult>> GetDetailsAsync(
        Guid id,
        ClaimsPrincipal principal,
        IMemoryDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.IdOf(principal);
        var memory = await query.GetAsync(id, accountId, cancellationToken);
        return memory is null
            ? Problems.From(MemoryErrors.NoAccess)
            : TypedResults.Ok(new MemoryDetailsResponse(
                memory.Id,
                memory.PlaceName,
                memory.Latitude,
                memory.Longitude,
                memory.DatePrecision,
                memory.DateFrom,
                memory.DateTo,
                memory.AuthorAccountId == accountId,
                memory.Description,
                memory.DescriptionIsAi,
                [.. memory.Photos.Select(photo => new MemoryPhotoResponse(photo.Id, photo.Url))]));
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> TrashAsync(
        Guid id,
        ClaimsPrincipal principal,
        TrashMemoryHandler handler,
        CancellationToken cancellationToken)
    {
        var trashed = await handler.HandleAsync(new TrashMemory(new MemoryId(id), CurrentAccount.IdOf(principal)), cancellationToken);
        return trashed.IsFailure ? Problems.From(trashed.Error) : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RequestDataDeletionAsync(
        Guid id,
        RequestDataDeletionRequest request,
        ClaimsPrincipal principal,
        RequestDataDeletionHandler handler,
        CancellationToken cancellationToken)
    {
        var deleted = await handler.HandleAsync(new RequestDataDeletion(new MemoryId(id), CurrentAccount.IdOf(principal), request.Confirmed), cancellationToken);
        return deleted.IsFailure ? Problems.From(deleted.Error) : TypedResults.NoContent();
    }

    private static async Task<Ok<IReadOnlyList<TrashedMemoryResponse>>> GetTrashAsync(
        ClaimsPrincipal principal,
        ITrashQuery query,
        CancellationToken cancellationToken)
    {
        var trashed = await query.GetAsync(CurrentAccount.IdOf(principal), cancellationToken);
        return TypedResults.Ok<IReadOnlyList<TrashedMemoryResponse>>(
            [.. trashed.Select(item => new TrashedMemoryResponse(item.Id, item.PlaceName, item.DateFrom, item.Description, item.Photos, item.TrashedAt, item.DaysLeft))]);
    }

    private static async Task<Results<Ok<IReadOnlyList<MyMemoryResponse>>, ProblemHttpResult>> GetLegacyAsync(
        Guid ownerAccountId,
        ClaimsPrincipal principal,
        IAccessReader access,
        ITrimMaterialQuery query,
        CancellationToken cancellationToken)
    {
        if (!(await access.LegacyArchivesAsync(CurrentAccount.IdOf(principal), cancellationToken)).Contains(ownerAccountId))
        {
            return Problems.From(MemoryErrors.NoAccess);
        }

        var items = await query.OfAccountAsync(ownerAccountId, cancellationToken);
        return TypedResults.Ok<IReadOnlyList<MyMemoryResponse>>([.. items.Select(item => new MyMemoryResponse(item.MemoryId, item.PlaceName, item.DateFrom))]);
    }

    private static async Task<Ok<IReadOnlyList<TrimMaterialResponse>>> GetTrimMaterialAsync(
        ClaimsPrincipal principal,
        ITrimMaterialQuery query,
        CancellationToken cancellationToken)
    {
        var items = await query.OfAccountAsync(CurrentAccount.IdOf(principal), cancellationToken);
        return TypedResults.Ok<IReadOnlyList<TrimMaterialResponse>>(
            [.. items.Select(item => new TrimMaterialResponse(item.MemoryId, item.PlaceName, item.DateFrom, item.SizeBytes))]);
    }

    private static async Task<Ok<IReadOnlyList<MyMemoryResponse>>> GetMineAsync(
        ClaimsPrincipal principal,
        IMyMemoriesQuery query,
        CancellationToken cancellationToken)
    {
        var mine = await query.GetAsync(CurrentAccount.IdOf(principal), cancellationToken);
        return TypedResults.Ok<IReadOnlyList<MyMemoryResponse>>([.. mine.Select(item => new MyMemoryResponse(item.Id, item.PlaceName, item.DateFrom))]);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RestoreAsync(
        Guid id,
        ClaimsPrincipal principal,
        RestoreMemoryHandler handler,
        CancellationToken cancellationToken)
    {
        var restored = await handler.HandleAsync(new RestoreMemory(new MemoryId(id), CurrentAccount.IdOf(principal)), cancellationToken);
        return restored.IsFailure ? Problems.From(restored.Error) : TypedResults.NoContent();
    }

    private static async Task<Results<Ok<IReadOnlyList<MemoryPointResponse>>, ProblemHttpResult>> GetMapAsync(
        string? from,
        string? to,
        ClaimsPrincipal principal,
        IMemoryMapQuery query,
        CancellationToken cancellationToken)
    {
        if (!TryParseDate(from, out var first) || !TryParseDate(to, out var last))
        {
            return Problems.From(MemoryErrors.InvalidDate);
        }

        var points = await query.GetAsync(CurrentAccount.IdOf(principal), first, last, cancellationToken);
        return TypedResults.Ok<IReadOnlyList<MemoryPointResponse>>(
            [.. points.Select(point => new MemoryPointResponse(point.Id, point.Latitude, point.Longitude))]);
    }

    private static bool TryParseDate(string? value, out DateOnly? date)
    {
        date = null;
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        if (!DateOnly.TryParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsed))
        {
            return false;
        }

        date = parsed;
        return true;
    }
}

/// <param name="Text">The description.</param>
/// <param name="AiDraftId">The AI draft it started from, if any.</param>
internal sealed record ApproveDescriptionRequest(string? Text, Guid? AiDraftId);

/// <param name="IsMine">The person asking is the author.</param>
/// <param name="DescriptionIsAi">The description started as an AI draft: shown with the AI marker.</param>
internal sealed record MemoryDetailsResponse(
    Guid Id,
    string PlaceName,
    double Latitude,
    double Longitude,
    string DatePrecision,
    string DateFrom,
    string DateTo,
    bool IsMine,
    string? Description,
    bool DescriptionIsAi,
    IReadOnlyList<MemoryPhotoResponse> Photos);

/// <param name="Url">Signed for 5 minutes.</param>
internal sealed record MemoryPhotoResponse(Guid Id, Uri Url);

/// <param name="Confirmed">The person confirmed that this cannot be undone.</param>
internal sealed record RequestDataDeletionRequest(bool Confirmed);

/// <param name="DaysLeft">Whole days, rounded up.</param>
internal sealed record TrashedMemoryResponse(Guid Id, string PlaceName, string DateFrom, string? Description, int Photos, DateTimeOffset TrashedAt, int DaysLeft);

internal sealed record MyMemoryResponse(Guid Id, string PlaceName, string DateFrom);

internal sealed record MemoryPointResponse(Guid Id, double Latitude, double Longitude);

internal sealed record StartUploadRequest(string? ContentType);

/// <param name="Key">Pass it on when adding the photo.</param>
/// <param name="Url">Send the file here with PUT and this content type.</param>
internal sealed record UploadResponse(string Key, Uri Url, string ContentType);

/// <param name="PhotoId">A UUID the client made for this photo; the idempotency key.</param>
/// <param name="UploadKey">The key from starting the upload.</param>
internal sealed record AddPhotoRequest(Guid PhotoId, string? UploadKey);

/// <param name="Id">A UUID the client made for this memory; the idempotency key.</param>
/// <param name="PlaceReference">The reference of a place from Places.</param>
/// <param name="DatePrecision">Day, Month, Year or Period.</param>
/// <param name="DateFrom">yyyy-MM-dd.</param>
/// <param name="DateTo">yyyy-MM-dd; only for a period.</param>
internal sealed record CreateMemoryRequest(Guid Id, string? PlaceReference, string? DatePrecision, string? DateFrom, string? DateTo);

internal sealed record CreatedMemoryResponse(Guid Id);

internal sealed record TrimMaterialResponse(Guid MemoryId, string PlaceName, string DateFrom, long SizeBytes);

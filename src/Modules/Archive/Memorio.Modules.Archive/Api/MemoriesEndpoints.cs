using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Archive.Application.Commands.AddPhoto;
using Memorio.Modules.Archive.Application.Commands.ApproveDescription;
using Memorio.Modules.Archive.Application.Commands.CreateMemory;
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
}

/// <param name="Text">The description.</param>
/// <param name="AiDraftId">The AI draft it started from, if any.</param>
internal sealed record ApproveDescriptionRequest(string? Text, Guid? AiDraftId);

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

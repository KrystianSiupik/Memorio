using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Archive.Application.Commands.CreateMemory;
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
}

/// <param name="Id">A UUID the client made for this memory; the idempotency key.</param>
/// <param name="PlaceReference">The reference of a place from Places.</param>
/// <param name="DatePrecision">Day, Month, Year or Period.</param>
/// <param name="DateFrom">yyyy-MM-dd.</param>
/// <param name="DateTo">yyyy-MM-dd; only for a period.</param>
internal sealed record CreateMemoryRequest(Guid Id, string? PlaceReference, string? DatePrecision, string? DateFrom, string? DateTo);

internal sealed record CreatedMemoryResponse(Guid Id);

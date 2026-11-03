using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Places.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Application.Commands.CreateMemory;

/// <summary>C12: a person records a memory. The id comes from the client (offline queue): sending it again is the same memory.</summary>
internal sealed record CreateMemory(Guid Id, Guid AuthorAccountId, string? PlaceReference, string? DatePrecision, string? DateFrom, string? DateTo);

internal sealed class CreateMemoryHandler(IMemories memories, IPlaceReader places, ITermsReader terms, TimeProvider clock)
{
    public async Task<Result<Memory>> HandleAsync(CreateMemory command, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        var date = MemoryDate.Create(command.DatePrecision, command.DateFrom, command.DateTo, DateOnly.FromDateTime(now.UtcDateTime));
        if (date.IsFailure)
        {
            return date.Error;
        }

        if (!await terms.HasAcceptedCurrentTermsAsync(command.AuthorAccountId, cancellationToken))
        {
            return MemoryErrors.TermsNotAccepted;
        }

        var place = await places.FindAsync(command.AuthorAccountId, command.PlaceReference ?? "", cancellationToken);
        if (place is null)
        {
            return MemoryErrors.PlaceNotFound;
        }

        var memory = await memories.AddOrFindAsync(
            Memory.Create(
                new MemoryId(command.Id),
                command.AuthorAccountId,
                new MemoryPlace(place.PlaceReference, place.Name, place.Latitude, place.Longitude),
                date.Value,
                now),
            cancellationToken);
        return memory.AuthorAccountId == command.AuthorAccountId ? memory : MemoryErrors.IdTaken;
    }
}

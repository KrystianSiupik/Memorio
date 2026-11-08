namespace Memorio.Modules.Archive.Contracts;

/// <summary>
/// Read port for AiNarration (R8): what a summary may tell of these memories, only of those in use (not in the trash).
/// Nothing else: no photos, no coordinates (S2:C5: they never reach the provider).
/// </summary>
public interface IMemoryFacts
{
    Task<IReadOnlyList<MemoryFact>> FactsAsync(IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken);
}

/// <param name="DatePrecision">Day, Month, Year or Period.</param>
/// <param name="Time">HH:mm, when the person gave the hour.</param>
/// <param name="Description">The approved description, if any.</param>
public sealed record MemoryFact(
    Guid MemoryId,
    Guid AuthorAccountId,
    string PlaceName,
    string DatePrecision,
    DateOnly DateFrom,
    DateOnly DateTo,
    string? Time,
    string? Description);

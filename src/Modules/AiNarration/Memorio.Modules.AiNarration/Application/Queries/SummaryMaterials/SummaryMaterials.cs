namespace Memorio.Modules.AiNarration.Application.Queries.SummaryMaterials;

/// <summary>
/// R8: what a summary of this album and period may be made of, for this person, now (S2:C5). Only memories they may read
/// (Access), from the period, of authors who gave the AI consent for this album. What is left out is counted, so the
/// summary says it was made from part of the material ("2 people's memories left out").
/// </summary>
internal interface ISummaryMaterialsQuery
{
    Task<SummaryMaterials> ForAsync(Guid accountId, Guid albumId, DateOnly from, DateOnly to, CancellationToken cancellationToken);
}

/// <param name="ReadAt">When the materials were read: the artifact checks withdrawals from then on (AG9, D-023).</param>
internal sealed record SummaryMaterials(IReadOnlyList<MaterialFact> Facts, int LeftOutMemories, int LeftOutPeople, DateTimeOffset ReadAt);

internal sealed record MaterialFact(
    Guid MemoryId,
    Guid AuthorAccountId,
    string PlaceName,
    string DatePrecision,
    DateOnly DateFrom,
    DateOnly DateTo,
    string? Time,
    string? Description);

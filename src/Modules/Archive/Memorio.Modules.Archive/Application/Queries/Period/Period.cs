namespace Memorio.Modules.Archive.Application.Queries.Period;

/// <summary>
/// R7: what the person may see in a period, in order (S2:C1): a memory belongs to every period its dates touch, so a
/// trip over New Year is in both years. Within a day, the order the person added them (D-300).
/// </summary>
internal interface IPeriodQuery
{
    Task<IReadOnlyList<PeriodMemory>> GetAsync(Guid accountId, DateOnly from, DateOnly to, CancellationToken cancellationToken);

    /// <summary>For Album (C71): only the person's own memories, the ones they may bring to an album (D-248).</summary>
    Task<IReadOnlyList<Guid>> OwnAsync(Guid accountId, DateOnly from, DateOnly to, CancellationToken cancellationToken);
}

/// <param name="PhotoUrls">The first photos, signed for this read (5 minutes).</param>
internal sealed record PeriodMemory(
    Guid Id,
    string PlaceName,
    double Latitude,
    double Longitude,
    string DatePrecision,
    string DateFrom,
    string DateTo,
    string? Description,
    bool IsMine,
    int Photos,
    IReadOnlyList<Uri> PhotoUrls);

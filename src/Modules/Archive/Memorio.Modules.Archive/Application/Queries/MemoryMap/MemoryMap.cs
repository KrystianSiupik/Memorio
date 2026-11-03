namespace Memorio.Modules.Archive.Application.Queries.MemoryMap;

/// <summary>
/// R11: one point per memory the person may see. For now their own: albums come with E3 and E4 (ADR 0011).
/// Dates filter by overlap, so "July" finds a trip from June to August.
/// </summary>
internal interface IMemoryMapQuery
{
    Task<IReadOnlyList<MemoryPoint>> GetAsync(Guid accountId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken);
}

internal sealed record MemoryPoint(Guid Id, double Latitude, double Longitude);

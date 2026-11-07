namespace Memorio.Modules.Archive.Application.Queries.MemoryMap;

/// <summary>
/// R11: one point per memory the person may see. For now their own: albums come with E3 and E4 (ADR 0011).
/// Dates filter by overlap, so "July" finds a trip from June to August.
/// </summary>
internal interface IMemoryMapQuery
{
    /// <param name="withPhotos">MEM-E13-PLAT-05: also the first photo of each memory, signed, for photo points on the map.</param>
    Task<IReadOnlyList<MemoryPoint>> GetAsync(Guid accountId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken, bool withPhotos = false);
}

/// <param name="PhotoUrl">The first photo added, signed for 5 minutes; only when asked for, and null without photos.</param>
internal sealed record MemoryPoint(Guid Id, double Latitude, double Longitude, Uri? PhotoUrl = null);

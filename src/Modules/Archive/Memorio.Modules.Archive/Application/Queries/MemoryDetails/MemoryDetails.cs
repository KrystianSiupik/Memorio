namespace Memorio.Modules.Archive.Application.Queries.MemoryDetails;

/// <summary>R12: everything about one memory. Null when the person may not see it: not visible and not there look the same.</summary>
internal interface IMemoryDetailsQuery
{
    Task<MemoryDetails?> GetAsync(Guid memoryId, Guid accountId, CancellationToken cancellationToken);
}

internal sealed record MemoryDetails(
    Guid Id,
    string PlaceName,
    double Latitude,
    double Longitude,
    string DatePrecision,
    string DateFrom,
    string DateTo,
    string? Time,
    Guid AuthorAccountId,
    string? Description,
    bool DescriptionIsAi,
    IReadOnlyList<MemoryPhoto> Photos);

internal sealed record MemoryPhoto(Guid Id, Uri Url);

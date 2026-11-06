namespace Memorio.Modules.Archive.Application.Queries.MemoryDetails;

/// <summary>R12: everything about one memory. Null when the person may not see it: not visible and not there look the same.</summary>
internal interface IMemoryDetailsQuery
{
    Task<MemoryDetails?> GetAsync(Guid memoryId, Guid accountId, CancellationToken cancellationToken);

    /// <summary>MEM-E12-PLAT-01: what the map shows on hovering a point, with the same access as the details.</summary>
    Task<MemoryPreview?> PreviewAsync(Guid memoryId, Guid accountId, CancellationToken cancellationToken);
}

/// <param name="Excerpt">The start of the description, cut at a word, with "…" when it goes on.</param>
/// <param name="PhotoUrl">The first photo added, signed for 5 minutes.</param>
internal sealed record MemoryPreview(Guid Id, string PlaceName, string DatePrecision, string DateFrom, string DateTo, string? Time, string? Excerpt, int Photos, Uri? PhotoUrl)
{
    public const int ExcerptLength = 140;

    public static string? ExcerptOf(string? description)
    {
        var text = description?.Trim().ReplaceLineEndings(" ");
        if (string.IsNullOrEmpty(text) || text.Length <= ExcerptLength)
        {
            return string.IsNullOrEmpty(text) ? null : text;
        }

        var cut = text.LastIndexOf(' ', ExcerptLength);
        return text[..(cut > ExcerptLength / 2 ? cut : ExcerptLength)].TrimEnd(' ', ',', '.', ';', ':') + "…";
    }
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

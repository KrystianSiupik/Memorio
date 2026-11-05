using System.Globalization;
using Memorio.Modules.Archive.Domain.Memories;

namespace Memorio.Modules.Archive.Application.Queries.TrimMaterial;

/// <summary>
/// What a trim can take, oldest first. For an account: its author's active memories no album pays for (D-264). For an
/// album: its active memories, which only leave the album and stay with their authors (D-290).
/// </summary>
internal interface ITrimMaterialQuery
{
    Task<IReadOnlyList<TrimMaterialItem>> OfAccountAsync(Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TrimMaterialItem>> OfAlbumAsync(Guid albumId, CancellationToken cancellationToken);
}

internal sealed record TrimMaterialItem(Guid MemoryId, string PlaceName, string DateFrom, long SizeBytes)
{
    public TrimCandidate ToCandidate() => new(MemoryId, DateOnly.Parse(DateFrom, CultureInfo.InvariantCulture), SizeBytes);
}

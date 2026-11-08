using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Provenance.Contracts;

/// <summary>Write port for AiNarration and later makers of artifacts: C154, record what an artifact was made from.</summary>
public interface IArtifactRegistry
{
    /// <summary>The same artifact again succeeds and changes nothing; wrong input or another artifact's id fails.</summary>
    Task<Result> RegisterAsync(NewArtifact artifact, CancellationToken cancellationToken);
}

/// <param name="Id">The maker's own id of the artifact (e.g. the summary's).</param>
/// <param name="Kind">One of <see cref="ArtifactKinds"/>.</param>
/// <param name="OwnerAccountId">The person it was made for.</param>
/// <param name="AlbumId">The album it was made from, if any.</param>
/// <param name="MaterialsReadAt">When the maker read the materials: what was withdrawn after it invalidates the artifact at once (AG9, D-023).</param>
public sealed record NewArtifact(
    Guid Id,
    string Kind,
    Guid OwnerAccountId,
    Guid? AlbumId,
    DateTimeOffset MaterialsReadAt,
    IReadOnlyCollection<ArtifactSourceItem> Sources);

/// <param name="AlbumId">The album through which the memory was used; null for one's own private memory.</param>
public sealed record ArtifactSourceItem(Guid MemoryId, Guid AuthorAccountId, Guid? AlbumId);

public static class ArtifactKinds
{
    /// <summary>C82: an AI summary of a period.</summary>
    public const string PeriodSummary = "PeriodSummary";
}

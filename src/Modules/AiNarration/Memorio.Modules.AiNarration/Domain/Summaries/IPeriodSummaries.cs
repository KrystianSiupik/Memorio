namespace Memorio.Modules.AiNarration.Domain.Summaries;

/// <summary>The summaries; a save publishes what they raised through the outbox.</summary>
internal interface IPeriodSummaries
{
    Task<PeriodSummary?> FindAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>The valid summaries added to the album, the newest first.</summary>
    Task<IReadOnlyList<PeriodSummary>> InAlbumAsync(Guid albumId, CancellationToken cancellationToken);

    /// <summary>The new summary, or the one the same request saved first.</summary>
    Task<PeriodSummary> AddOrFindAsync(PeriodSummary summary, CancellationToken cancellationToken);

    /// <summary>False when somebody else saved first.</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}

namespace Memorio.Modules.Succession.Application.Queries;

/// <summary>R17: only the successor of the case asks; facts of the album come from Album at the edge.</summary>
internal interface ITakeoverQueries
{
    Task<TakeoverOfferRow?> FindAsync(Guid caseId, CancellationToken cancellationToken);

    /// <summary>What waits for this person's answer now.</summary>
    Task<IReadOnlyList<TakeoverOfferRow>> OpenOffersOfAsync(Guid accountId, DateTimeOffset now, CancellationToken cancellationToken);
}

internal sealed record TakeoverOfferRow(Guid Id, Guid AlbumId, Guid OwnerAccountId, Guid? SuccessorAccountId, string State, DateTimeOffset? AnswerBy);

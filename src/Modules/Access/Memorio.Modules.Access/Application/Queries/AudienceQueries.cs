namespace Memorio.Modules.Access.Application.Queries;

internal sealed record ViewerRow(Guid AccountId, DateTimeOffset EnteredAt);

internal interface IAudienceQueries
{
    /// <summary>R10: the caller has checked who is asking.</summary>
    Task<IReadOnlyList<ViewerRow>> ViewersAsync(Guid albumId, CancellationToken cancellationToken);

    Task<string?> LinkTokenAsync(Guid albumId, CancellationToken cancellationToken);

    /// <summary>The album behind a working link.</summary>
    Task<Guid?> AlbumOfTokenAsync(string token, CancellationToken cancellationToken);
}

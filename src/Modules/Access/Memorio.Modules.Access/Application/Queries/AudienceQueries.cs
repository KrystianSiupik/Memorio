namespace Memorio.Modules.Access.Application.Queries;

/// <param name="EnteredAt">In UTC, as Npgsql reads it.</param>
internal sealed record ViewerRow(Guid AccountId, DateTime EnteredAt);

internal interface IAudienceQueries
{
    /// <summary>R10: the caller has checked who is asking.</summary>
    Task<IReadOnlyList<ViewerRow>> ViewersAsync(Guid albumId, CancellationToken cancellationToken);

    /// <summary>Who is blocked: the caller has checked it is the owner.</summary>
    Task<IReadOnlyList<Guid>> BlockedAsync(Guid albumId, CancellationToken cancellationToken);

    Task<string?> LinkTokenAsync(Guid albumId, CancellationToken cancellationToken);

    /// <summary>The album behind a working link.</summary>
    Task<Guid?> AlbumOfTokenAsync(string token, CancellationToken cancellationToken);
}

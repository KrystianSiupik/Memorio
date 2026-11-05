using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Access.Contracts;

/// <summary>E66: the person no longer reads the album.</summary>
public sealed record AccessRevokedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId, int Sequence) : IIntegrationEvent;

/// <summary>E67: the person may not come into the album in any way.</summary>
public sealed record UserBlockedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId, int Sequence) : IIntegrationEvent;

/// <summary>Read port for Album: a blocked person cannot take a seat (S2:C11, D-022).</summary>
public interface IBlockList
{
    Task<bool> IsBlockedAsync(Guid albumId, Guid accountId, CancellationToken cancellationToken);
}

/// <summary>Write port for rules of other modules (C65). False when a block stands in the way.</summary>
public interface IReadGrants
{
    Task<bool> GrantAsync(Guid albumId, Guid accountId, CancellationToken cancellationToken);
}

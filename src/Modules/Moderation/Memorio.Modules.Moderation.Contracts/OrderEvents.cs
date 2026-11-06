using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Moderation.Contracts;

/// <summary>E115: an order to remove content came in; P19 hides it at once (L11: within an hour).</summary>
public sealed record RemovalOrderReceivedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid OrderId, int Sequence) : IIntegrationEvent;

/// <summary>E116: an order to preserve or produce an account's data came in; P20 secures it first (L18).</summary>
public sealed record DataOrderReceivedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid OrderId, int Sequence) : IIntegrationEvent;

/// <summary>E117: nothing of the account goes for good while the order stands; asked through <see cref="ILegalHolds"/>.</summary>
public sealed record AccountDataSecuredEvent(Guid EventId, DateTimeOffset OccurredAt, Guid OrderId, Guid AccountId, int Sequence) : IIntegrationEvent;

/// <summary>An order hid content; the author hears it, unless the authority asked otherwise (<paramref name="Confidential"/>).</summary>
public sealed record RemovalOrderExecutedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid OrderId, Guid? AuthorAccountId, bool Confidential, int Sequence) : IIntegrationEvent;

/// <summary>
/// Read port for Archive and Account (C114): while an order of an authority holds the account, nothing of it goes for
/// good; what would go waits, unseen, until the hold ends. Asked at the moment of deleting, never cached.
/// </summary>
public interface ILegalHolds
{
    Task<bool> IsHeldAsync(Guid accountId, CancellationToken cancellationToken);
}

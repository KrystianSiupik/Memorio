namespace Memorio.SharedKernel.Messaging;

/// <summary>
/// Fact one module publishes for others (ADR 0006): the type lives in the publisher's Contracts, it is
/// stored in the outbox in the publisher's transaction and delivered by the worker at least once, so
/// every handler must be idempotent on <see cref="EventId"/> (SD6).
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAt { get; }
}

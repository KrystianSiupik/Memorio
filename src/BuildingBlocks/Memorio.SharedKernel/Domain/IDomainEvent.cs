namespace Memorio.SharedKernel.Domain;

/// <summary>Something that happened inside one aggregate. Handlers must be idempotent on <see cref="EventId"/>.</summary>
public interface IDomainEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAt { get; }
}

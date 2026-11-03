using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Limits.Domain.Pools;

/// <summary>E150: the space is taken.</summary>
internal sealed record UnitsReserved(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, Guid OperationKey, long Bytes) : IDomainEvent;

/// <summary>E152: the space is back; only when this call gave it back.</summary>
internal sealed record UnitsReleased(Guid EventId, DateTimeOffset OccurredAt, Guid OperationKey) : IDomainEvent;

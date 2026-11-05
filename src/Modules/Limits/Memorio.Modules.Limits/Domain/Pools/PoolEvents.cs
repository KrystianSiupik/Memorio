using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Limits.Domain.Pools;

/// <summary>E150: the space is taken.</summary>
internal sealed record UnitsReserved(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, Guid OperationKey, long Bytes) : IDomainEvent;

/// <summary>E153: only when this limit was newer than the one set (D-016).</summary>
internal sealed record PoolLimitSet(Guid EventId, DateTimeOffset OccurredAt, Guid PoolId, long LimitBytes, long Sequence) : IDomainEvent;

/// <summary>E152: the space is back; only when this call gave it back.</summary>
internal sealed record UnitsReleased(Guid EventId, DateTimeOffset OccurredAt, Guid OperationKey) : IDomainEvent;

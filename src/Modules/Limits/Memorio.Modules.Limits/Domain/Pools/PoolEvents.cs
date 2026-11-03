using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Limits.Domain.Pools;

/// <summary>E150: the space is taken.</summary>
internal sealed record UnitsReserved(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, Guid OperationKey, long Bytes) : IDomainEvent;

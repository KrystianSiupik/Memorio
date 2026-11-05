using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Plans.Domain.Cycle;

/// <summary>E94.</summary>
internal sealed record PlanActivated(Guid EventId, DateTimeOffset OccurredAt, EntitledResource Resource, PoolTerms Terms, int Sequence) : IDomainEvent;

/// <summary>E95.</summary>
internal sealed record PlanWentReadOnly(Guid EventId, DateTimeOffset OccurredAt, EntitledResource Resource, PoolTerms Terms, int Sequence) : IDomainEvent;

/// <summary>E97.</summary>
internal sealed record TrimWarningDue(Guid EventId, DateTimeOffset OccurredAt, EntitledResource Resource, int DaysBefore, DateTimeOffset TrimAt, int Round) : IDomainEvent;

/// <summary>E100.</summary>
internal sealed record TrimHeld(Guid EventId, DateTimeOffset OccurredAt, EntitledResource Resource, int DaysBefore, int Round, int Sequence) : IDomainEvent;

/// <summary>E101; stays inside the module.</summary>
internal sealed record MaterialChosen(Guid EventId, DateTimeOffset OccurredAt, EntitledResource Resource) : IDomainEvent;

/// <summary>E103.</summary>
internal sealed record TrimOrdered(Guid EventId, DateTimeOffset OccurredAt, EntitledResource Resource, PoolTerms Terms, IReadOnlyList<Guid> KeptMemoryIds, int Sequence) : IDomainEvent;

/// <summary>E104; stays inside the module.</summary>
internal sealed record SubscriptionSynchronized(Guid EventId, DateTimeOffset OccurredAt, EntitledResource Resource) : IDomainEvent;

/// <summary>E171.</summary>
internal sealed record PaidWhileOwnerless(Guid EventId, DateTimeOffset OccurredAt, EntitledResource Resource, int Sequence) : IDomainEvent;

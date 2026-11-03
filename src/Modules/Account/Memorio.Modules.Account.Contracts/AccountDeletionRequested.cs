using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Account.Contracts;

/// <summary>E4: a 30-day window for deleting this account opened. <paramref name="Sequence"/> grows with every window of the account (D-013).</summary>
public sealed record AccountDeletionRequested(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid AccountId,
    int Sequence,
    DateTimeOffset WindowEndsAt) : IIntegrationEvent;

using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Account.Contracts;

/// <summary>
/// E7: this account is gone for good; modules holding something of this person react. Only the effect,
/// never why: the window it came from is its <paramref name="Sequence"/>, for ordering (D-013).
/// </summary>
public sealed record AccountDeleted(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, int Sequence) : IIntegrationEvent;

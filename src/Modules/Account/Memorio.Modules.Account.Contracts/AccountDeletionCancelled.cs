using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Account.Contracts;

/// <summary>E8: the window with this <paramref name="Sequence"/> was cancelled; the account stays.</summary>
public sealed record AccountDeletionCancelled(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, int Sequence) : IIntegrationEvent;

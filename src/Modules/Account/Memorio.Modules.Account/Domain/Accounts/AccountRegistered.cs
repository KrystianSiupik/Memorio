using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Account.Domain.Accounts;

/// <summary>An account was created (E1). Nobody outside the module reacts to it yet, so it does not go through the outbox.</summary>
internal sealed record AccountRegistered(Guid EventId, DateTimeOffset OccurredAt, AccountId AccountId) : IDomainEvent;

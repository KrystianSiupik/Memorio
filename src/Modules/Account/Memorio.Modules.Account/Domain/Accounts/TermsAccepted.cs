using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Account.Domain.Accounts;

/// <summary>A person accepted a version of the terms (E3).</summary>
internal sealed record TermsAccepted(Guid EventId, DateTimeOffset OccurredAt, AccountId AccountId, string Version) : IDomainEvent;

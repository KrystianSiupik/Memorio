using Memorio.Modules.Account.Domain.Accounts;
using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Account.Domain.Deletions;

/// <summary>E4: the window opened.</summary>
internal sealed record DeletionRequested(
    Guid EventId,
    DateTimeOffset OccurredAt,
    AccountId AccountId,
    int Sequence,
    DateTimeOffset WindowEndsAt) : IDomainEvent;

using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Account.Tests;

/// <summary>In memory, keeping the events the real repository would put in the outbox.</summary>
internal sealed class FakeDeletions : IAccountDeletions
{
    public AccountDeletion? Stored { get; private set; }

    /// <summary>The next save loses to this deletion, as to a parallel decision.</summary>
    public AccountDeletion? LosesTo { get; set; }

    public List<IDomainEvent> Saved { get; } = [];

    public Task<AccountDeletion?> FindAsync(AccountId accountId, CancellationToken cancellationToken) =>
        Task.FromResult(Stored?.Id == accountId ? Stored : null);

    public void Add(AccountDeletion deletion)
    {
    }

    public Task<bool> SaveAsync(AccountDeletion deletion, CancellationToken cancellationToken)
    {
        if (LosesTo is not null)
        {
            Stored = LosesTo;
            LosesTo = null;
            return Task.FromResult(false);
        }

        Stored = deletion;
        Saved.AddRange(deletion.DomainEvents);
        deletion.ClearDomainEvents();
        return Task.FromResult(true);
    }
}

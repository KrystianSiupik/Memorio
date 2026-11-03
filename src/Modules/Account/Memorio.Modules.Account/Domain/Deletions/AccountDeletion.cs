using Memorio.Modules.Account.Domain.Accounts;
using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Domain.Deletions;

/// <summary>
/// AG11: one window of 30 days has one result, deleted or cancelled (D-024). One per account; a cancelled
/// one can open again, as a new window with a higher <see cref="Sequence"/>. The state is a concurrency
/// token, so saving a change is a conditional UPDATE and only one of two decisions gets through.
/// </summary>
internal sealed class AccountDeletion : AggregateRoot<AccountId>
{
    public static readonly TimeSpan Window = TimeSpan.FromDays(30);

    private AccountDeletion(AccountId id, DateTimeOffset requestedAt, DateTimeOffset windowEndsAt, bool consentsToAlbumExpiry)
        : base(id)
    {
        RequestedAt = requestedAt;
        WindowEndsAt = windowEndsAt;
        ConsentsToAlbumExpiry = consentsToAlbumExpiry;
    }

    public DeletionState State { get; private set; } = DeletionState.Pending;

    public int Sequence { get; private set; } = 1;

    public DateTimeOffset RequestedAt { get; private set; }

    public DateTimeOffset WindowEndsAt { get; private set; }

    /// <summary>The person agreed that their albums without a successor expire with the account (S2:B3).</summary>
    public bool ConsentsToAlbumExpiry { get; private set; }

    public static AccountDeletion Request(AccountId accountId, DateTimeOffset now, bool consentsToAlbumExpiry)
    {
        var deletion = new AccountDeletion(accountId, now, now + Window, consentsToAlbumExpiry);
        deletion.RaiseRequested(now);
        return deletion;
    }

    /// <summary>A new window after a cancelled one. While one is open nothing changes: the request was already heard.</summary>
    public Result RequestAgain(DateTimeOffset now, bool consentsToAlbumExpiry)
    {
        switch (State)
        {
            case DeletionState.Pending:
                return Result.Success();
            case DeletionState.Completed:
                return AccountErrors.DeletionAlreadyDecided;
            default:
                State = DeletionState.Pending;
                Sequence++;
                RequestedAt = now;
                WindowEndsAt = now + Window;
                ConsentsToAlbumExpiry = consentsToAlbumExpiry;
                RaiseRequested(now);
                return Result.Success();
        }
    }

    private void RaiseRequested(DateTimeOffset now) =>
        Raise(new DeletionRequested(Guid.CreateVersion7(), now, Id, Sequence, WindowEndsAt));
}

using Memorio.Modules.Account.Application.Commands.CancelAccountDeletion;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E1-KONTO-05: the rules of C5 and of AG11, without a database.</summary>
public sealed class CancelAccountDeletionTests
{
    private static readonly DateTimeOffset Requested = new(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);
    private static readonly AccountId Account = new(Guid.CreateVersion7());

    private readonly FakeDeletions _deletions = new();

    [Fact]
    public async Task During_the_window_the_account_stays_and_gives_the_event()
    {
        var deletion = await StoredAsync();

        var cancelled = await HandleAsync(Requested.AddDays(29));

        Assert.True(cancelled.IsSuccess);
        Assert.Equal(DeletionState.Cancelled, deletion.State);
        var happened = Assert.IsType<DeletionCancelled>(Assert.Single(_deletions.Saved));
        Assert.Equal(1, happened.Sequence);
    }

    [Fact]
    public async Task At_the_end_of_the_window_it_is_too_late()
    {
        var deletion = await StoredAsync();

        var refused = await HandleAsync(deletion.WindowEndsAt);

        Assert.Equal(AccountErrors.DeletionAlreadyDecided, refused.Error);
        Assert.Equal(DeletionState.Pending, deletion.State);
        Assert.Empty(_deletions.Saved);
    }

    [Fact]
    public async Task After_the_deletion_it_is_too_late()
    {
        var deletion = await StoredAsync();
        deletion.Complete(1, deletion.WindowEndsAt);

        var refused = await HandleAsync(Requested.AddDays(1));

        Assert.Equal(AccountErrors.DeletionAlreadyDecided, refused.Error);
    }

    [Fact]
    public async Task Cancelling_again_changes_nothing()
    {
        await StoredAsync();
        await HandleAsync(Requested.AddDays(1));

        var again = await HandleAsync(Requested.AddDays(2));

        Assert.True(again.IsSuccess);
        Assert.Single(_deletions.Saved);
    }

    [Fact]
    public async Task Without_a_request_there_is_nothing_to_cancel()
    {
        var refused = await HandleAsync(Requested);

        Assert.Equal(AccountErrors.NoDeletionRequested, refused.Error);
    }

    [Fact]
    public void A_new_request_after_cancelling_opens_a_new_window_with_a_higher_sequence()
    {
        var deletion = AccountDeletion.Request(Account, Requested, consentsToAlbumExpiry: false);
        deletion.Cancel(Requested.AddDays(1));
        deletion.ClearDomainEvents();

        deletion.RequestAgain(Requested.AddDays(2), consentsToAlbumExpiry: false);

        Assert.Equal(DeletionState.Pending, deletion.State);
        Assert.Equal(2, deletion.Sequence);
        Assert.Equal(Requested.AddDays(32), deletion.WindowEndsAt);
        Assert.Equal(2, Assert.IsType<DeletionRequested>(Assert.Single(deletion.DomainEvents)).Sequence);
    }

    private async Task<AccountDeletion> StoredAsync()
    {
        var deletion = AccountDeletion.Request(Account, Requested, consentsToAlbumExpiry: false);
        await _deletions.SaveAsync(deletion, TestContext.Current.CancellationToken);
        _deletions.Saved.Clear();
        return deletion;
    }

    private Task<SharedKernel.Results.Result> HandleAsync(DateTimeOffset now) =>
        new CancelAccountDeletionHandler(_deletions, new FixedClock(now))
            .HandleAsync(new CancelAccountDeletion(Account), TestContext.Current.CancellationToken);

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}

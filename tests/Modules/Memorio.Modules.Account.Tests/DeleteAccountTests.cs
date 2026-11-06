using Memorio.Modules.Account.Application.Commands.DeleteAccount;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E1-KONTO-04: the rules of C4 and of AG11, without a database.</summary>
public sealed class DeleteAccountTests
{
    private static readonly DateTimeOffset Requested = new(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset WindowEnd = Requested.AddDays(30);
    private static readonly AccountId Account = new(Guid.CreateVersion7());

    private readonly FakeDeletions _deletions = new();
    private readonly FakeLegalHolds _holds = new();

    [Fact]
    public async Task At_the_end_of_the_window_the_account_goes_and_gives_the_event()
    {
        var deletion = await StoredAsync();

        var deleted = await HandleAsync(sequence: 1, WindowEnd);

        Assert.True(deleted.IsSuccess);
        Assert.Equal(DeletionState.Completed, deletion.State);
        var completed = Assert.IsType<DeletionCompleted>(Assert.Single(_deletions.Saved));
        Assert.Equal(Account, completed.AccountId);
        Assert.Equal(1, completed.Sequence);
        Assert.Equal(WindowEnd, completed.OccurredAt);
    }

    [Fact]
    public async Task Before_the_end_of_the_window_nothing_happens()
    {
        var deletion = await StoredAsync();

        var refused = await HandleAsync(sequence: 1, WindowEnd.AddTicks(-1));

        Assert.Equal(AccountErrors.DeletionWindowOpen, refused.Error);
        Assert.Equal(DeletionState.Pending, deletion.State);
        Assert.Empty(_deletions.Saved);
    }

    [Fact]
    public async Task The_second_decision_about_a_window_is_refused()
    {
        await StoredAsync();
        await HandleAsync(sequence: 1, WindowEnd);

        var again = await HandleAsync(sequence: 1, WindowEnd);

        Assert.Equal(AccountErrors.DeletionAlreadyDecided, again.Error);
        Assert.Single(_deletions.Saved);
    }

    [Fact]
    public async Task The_deadline_of_an_earlier_window_changes_nothing()
    {
        var deletion = await StoredAsync();

        var stale = await HandleAsync(sequence: 0, WindowEnd);

        Assert.Equal(AccountErrors.DeletionAlreadyDecided, stale.Error);
        Assert.Equal(DeletionState.Pending, deletion.State);
    }

    [Fact]
    public async Task While_an_order_holds_the_data_the_account_waits_for_the_next_sweep()
    {
        await StoredAsync();
        _holds.Hold(Account.Value);

        Assert.Equal(AccountErrors.DataHeld, (await HandleAsync(sequence: 1, WindowEnd)).Error);
        Assert.Empty(_deletions.Saved);

        _holds.Release(Account.Value);
        Assert.True((await HandleAsync(sequence: 1, WindowEnd)).IsSuccess);
    }

    [Fact]
    public async Task Without_a_request_there_is_nothing_to_delete()
    {
        var refused = await HandleAsync(sequence: 1, WindowEnd);

        Assert.Equal(AccountErrors.NoDeletionRequested, refused.Error);
    }

    [Fact]
    public async Task Losing_to_a_parallel_decision_is_refused()
    {
        var deletion = await StoredAsync();
        _deletions.LosesTo = deletion;
        _deletions.Saved.Clear();

        var lost = await HandleAsync(sequence: 1, WindowEnd);

        Assert.Equal(AccountErrors.DeletionAlreadyDecided, lost.Error);
        Assert.Empty(_deletions.Saved);
    }

    private async Task<AccountDeletion> StoredAsync()
    {
        var deletion = AccountDeletion.Request(Account, Requested, consentsToAlbumExpiry: false);
        await _deletions.SaveAsync(deletion, TestContext.Current.CancellationToken);
        _deletions.Saved.Clear();
        return deletion;
    }

    private Task<SharedKernel.Results.Result> HandleAsync(int sequence, DateTimeOffset at) =>
        new DeleteAccountHandler(_deletions, _holds).HandleAsync(new DeleteAccount(Account, sequence, at), TestContext.Current.CancellationToken);
}

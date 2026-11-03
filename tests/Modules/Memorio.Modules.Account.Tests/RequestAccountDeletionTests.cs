using Memorio.Modules.Account.Application.Commands.RequestAccountDeletion;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.Modules.Album.Contracts;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E1-KONTO-03: the rules of C3, without a database.</summary>
public sealed class RequestAccountDeletionTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);
    private static readonly AccountId Account = new(Guid.CreateVersion7());

    private readonly FakeDeletions _deletions = new();
    private readonly FakeAlbums _albums = new();

    [Fact]
    public async Task A_request_opens_a_window_of_30_days_and_gives_the_event()
    {
        var windowEnd = await HandleAsync(consents: false);

        Assert.Equal(Now.AddDays(30), windowEnd.Value);
        var requested = Assert.IsType<DeletionRequested>(Assert.Single(_deletions.Saved));
        Assert.Equal(Account, requested.AccountId);
        Assert.Equal(1, requested.Sequence);
        Assert.Equal(Now.AddDays(30), requested.WindowEndsAt);
    }

    [Fact]
    public async Task Albums_without_a_successor_hold_it_back_and_nothing_is_stored()
    {
        _albums.WithoutSuccessor = 2;

        var held = await HandleAsync(consents: false);

        Assert.Equal(AccountErrors.AlbumWithoutSuccessor, held.Error);
        Assert.Empty(_deletions.Saved);
        Assert.Null(_deletions.Stored);
    }

    [Fact]
    public async Task Consent_to_the_albums_expiring_lets_it_through_and_is_kept()
    {
        _albums.WithoutSuccessor = 2;

        var windowEnd = await HandleAsync(consents: true);

        Assert.True(windowEnd.IsSuccess);
        Assert.True(_deletions.Stored?.ConsentsToAlbumExpiry);
    }

    [Fact]
    public async Task Asking_again_during_the_window_keeps_it_and_gives_no_second_event()
    {
        var first = await HandleAsync(consents: false);
        _albums.WithoutSuccessor = 1;

        var second = await HandleAsync(consents: false);

        Assert.Equal(first.Value, second.Value);
        Assert.Single(_deletions.Saved);
    }

    [Fact]
    public async Task Losing_the_race_to_the_same_request_answers_with_the_window_of_the_winner()
    {
        var winner = AccountDeletion.Request(Account, Now.AddMinutes(-1), consentsToAlbumExpiry: false);
        _deletions.LosesTo = winner;

        var windowEnd = await HandleAsync(consents: false);

        Assert.Equal(winner.WindowEndsAt, windowEnd.Value);
        Assert.Empty(_deletions.Saved);
    }

    private Task<SharedKernel.Results.Result<DateTimeOffset>> HandleAsync(bool consents) =>
        new RequestAccountDeletionHandler(_deletions, _albums, new FixedClock())
            .HandleAsync(new RequestAccountDeletion(Account, consents), TestContext.Current.CancellationToken);

    private sealed class FixedClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class FakeAlbums : IAccountAlbumsReader
    {
        public int WithoutSuccessor { get; set; }

        public Task<AccountAlbumsSummary> SummarizeAsync(Guid accountId, CancellationToken cancellationToken) =>
            Task.FromResult(new AccountAlbumsSummary(WithoutSuccessor, WithoutSuccessor, 0));
    }
}

using Memorio.Modules.Account.Application.Commands.AcceptTerms;
using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E1-KONTO-02: the rules of C2, without a database.</summary>
public sealed class AcceptTermsTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);
    private static readonly AccountId Account = new(Guid.CreateVersion7());

    [Fact]
    public async Task Accepting_the_current_version_records_it_and_gives_terms_accepted()
    {
        var accounts = new FakeRegistry();

        var accepted = await new AcceptTermsHandler(accounts, new FixedClock()).HandleAsync(new AcceptTerms(Account, TermsOfService.CurrentVersion));

        Assert.Equal(Account, accepted.Value.AccountId);
        Assert.Equal(TermsOfService.CurrentVersion, accepted.Value.Version);
        Assert.Equal(Now, accepted.Value.OccurredAt);
        Assert.Equal((Account, TermsOfService.CurrentVersion, Now), accounts.Recorded);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1999-01")]
    public async Task Any_other_version_is_refused_and_nothing_is_recorded(string? version)
    {
        var accounts = new FakeRegistry();

        var refused = await new AcceptTermsHandler(accounts, new FixedClock()).HandleAsync(new AcceptTerms(Account, version));

        Assert.Equal(AccountErrors.TermsVersionNotCurrent, refused.Error);
        Assert.Null(accounts.Recorded);
    }

    private sealed class FixedClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class FakeRegistry : IAccountRegistry
    {
        public (AccountId, string, DateTimeOffset)? Recorded { get; private set; }

        public Task<Result<AccountId>> CreateWithPasswordAsync(string email, string password, AgeDeclaration declaration) =>
            throw new NotSupportedException();

        public Task<Result> RecordTermsAcceptanceAsync(AccountId accountId, string version, DateTimeOffset acceptedAt)
        {
            Recorded = (accountId, version, acceptedAt);
            return Task.FromResult(Result.Success());
        }
    }
}

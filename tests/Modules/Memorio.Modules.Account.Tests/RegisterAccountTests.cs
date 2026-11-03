using Memorio.Modules.Account.Application.Commands.RegisterAccount;
using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E1-KONTO-01: the rules of C1, without a database.</summary>
public sealed class RegisterAccountTests
{
    private const string Email = "ola@example.com";
    private const string Password = "long-enough-password";

    private static readonly DateTimeOffset Now = new(2026, 11, 2, 12, 0, 0, TimeSpan.Zero);

    private readonly FakeRegistry _accounts = new();

    [Fact]
    public async Task A_valid_registration_stores_the_account_and_gives_account_registered()
    {
        var registered = await HandleAsync(new RegisterAccount(Email, Password, DeclaresMinimumAge: true));

        Assert.Equal(_accounts.Created, registered.Value.AccountId);
        Assert.Equal(Now, registered.Value.OccurredAt);
        Assert.NotEqual(Guid.Empty, registered.Value.EventId);
        Assert.Equal(Now, _accounts.Declaration?.DeclaredAt);
    }

    [Theory]
    [InlineData(Email, Password)]
    [InlineData("not-an-email", "short")]
    public async Task Without_the_declaration_nothing_is_stored(string email, string password)
    {
        var refused = await HandleAsync(new RegisterAccount(email, password, DeclaresMinimumAge: false));

        Assert.Equal(AccountErrors.AgeDeclarationRequired, refused.Error);
        Assert.Null(_accounts.Created);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    public async Task An_invalid_email_is_refused_and_nothing_is_stored(string? email)
    {
        var refused = await HandleAsync(new RegisterAccount(email, Password, DeclaresMinimumAge: true));

        Assert.Equal(AccountErrors.InvalidEmail, refused.Error);
        Assert.Null(_accounts.Created);
    }

    [Fact]
    public async Task An_email_longer_than_its_column_is_refused()
    {
        var refused = await HandleAsync(new RegisterAccount(new string('a', 245) + "@example.com", Password, DeclaresMinimumAge: true));

        Assert.Equal(AccountErrors.InvalidEmail, refused.Error);
        Assert.Null(_accounts.Created);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123456789")]
    public async Task A_password_under_the_minimum_is_refused_and_says_how_long_it_must_be(string? password)
    {
        var refused = await HandleAsync(new RegisterAccount(Email, password, DeclaresMinimumAge: true));

        Assert.Equal(AccountErrors.InvalidPassword, refused.Error);
        Assert.Contains("10", refused.Error.Description, StringComparison.Ordinal);
        Assert.Null(_accounts.Created);
    }

    [Fact]
    public async Task An_email_with_an_account_is_refused()
    {
        _accounts.Refusal = AccountErrors.EmailTaken;

        var refused = await HandleAsync(new RegisterAccount(Email, Password, DeclaresMinimumAge: true));

        Assert.Equal(AccountErrors.EmailTaken, refused.Error);
    }

    private Task<Result<AccountRegistered>> HandleAsync(RegisterAccount command) =>
        new RegisterAccountHandler(_accounts, new FixedClock()).HandleAsync(command);

    private sealed class FixedClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class FakeRegistry : IAccountRegistry
    {
        public Error? Refusal { get; set; }

        public AccountId? Created { get; private set; }

        public AgeDeclaration? Declaration { get; private set; }

        public Task<Result<AccountId>> CreateWithPasswordAsync(string email, string password, AgeDeclaration declaration)
        {
            if (Refusal is { } refusal)
            {
                return Task.FromResult<Result<AccountId>>(refusal);
            }

            Created = new AccountId(Guid.CreateVersion7());
            Declaration = declaration;
            return Task.FromResult<Result<AccountId>>(Created.Value);
        }

        public Task<Result> RecordTermsAcceptanceAsync(AccountId accountId, string version, DateTimeOffset acceptedAt) =>
            throw new NotSupportedException();
    }
}

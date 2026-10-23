using Memorio.Modules.Account.Domain.Accounts;

namespace Memorio.Modules.Account.Tests;

public sealed class AgeDeclarationTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Declaring_at_least_16_is_kept_with_its_time()
    {
        var declaration = AgeDeclaration.Declare(isAtLeastMinimumAge: true, Now);

        Assert.True(declaration.IsSuccess);
        Assert.Equal(Now, declaration.Value.DeclaredAt);
        Assert.Equal(16, AgeDeclaration.MinimumAge);
    }

    [Fact]
    public void Without_the_declaration_there_is_no_account()
    {
        var declaration = AgeDeclaration.Declare(isAtLeastMinimumAge: false, Now);

        Assert.Equal(AccountErrors.AgeDeclarationRequired, declaration.Error);
        Assert.Equal("Account.AgeDeclarationRequired", declaration.Error.Code);
    }
}

using System.ComponentModel.DataAnnotations;
using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Application.Commands.RegisterAccount;

/// <summary>C1: a person creates an account with an email and a password.</summary>
internal sealed record RegisterAccount(string? Email, string? Password, bool DeclaresMinimumAge)
{
    private const int MaximumEmailLength = 256;

    private static readonly EmailAddressAttribute EmailFormat = new();

    public Error? FindInputError()
    {
        if (string.IsNullOrWhiteSpace(Email) || Email.Length > MaximumEmailLength || !EmailFormat.IsValid(Email))
        {
            return AccountErrors.InvalidEmail;
        }

        return Password is null || Password.Length < AccountPassword.MinimumLength ? AccountErrors.InvalidPassword : null;
    }
}

/// <summary>Nothing is stored unless the person declares being at least 16 (E2) and the input is valid.</summary>
internal sealed class RegisterAccountHandler(IAccountRegistry accounts, TimeProvider clock)
{
    public async Task<Result<AccountRegistered>> HandleAsync(RegisterAccount command)
    {
        var now = clock.GetUtcNow();
        var declaration = AgeDeclaration.Declare(command.DeclaresMinimumAge, now);
        if (declaration.IsFailure)
        {
            return declaration.Error;
        }

        if (command.FindInputError() is { } inputError)
        {
            return inputError;
        }

        var account = await accounts.CreateWithPasswordAsync(command.Email!, command.Password!, declaration.Value);
        if (account.IsFailure)
        {
            return account.Error;
        }

        return new AccountRegistered(Guid.CreateVersion7(), now, account.Value);
    }
}

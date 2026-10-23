using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Domain.Accounts;

internal static class AccountErrors
{
    public static readonly Error AgeDeclarationRequired = Error.Validation(
        "Account.AgeDeclarationRequired",
        $"An account needs the declaration of being at least {AgeDeclaration.MinimumAge}.");

    public static readonly Error EmailTaken = Error.Conflict(
        "Account.EmailTaken",
        "An account with this email exists; sign in to it instead.");

    public static readonly Error InvalidPassword = Error.Validation(
        "Account.InvalidPassword",
        "The password does not meet the password rules.");

    public static readonly Error InvalidEmail = Error.Validation(
        "Account.InvalidEmail",
        "The email address is not valid.");

    public static readonly Error ProviderUnavailable = Error.NotFound(
        "Account.ProviderUnavailable",
        "This sign-in provider is not available.");
}

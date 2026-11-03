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
        $"The password needs at least {AccountPassword.MinimumLength} characters.");

    public static readonly Error InvalidEmail = Error.Validation(
        "Account.InvalidEmail",
        "The email address is not valid.");

    public static readonly Error TermsVersionNotCurrent = Error.Validation(
        "Account.TermsVersionNotCurrent",
        $"Only the current terms ({TermsOfService.CurrentVersion}) can be accepted.");

    public static readonly Error NotFound = Error.NotFound(
        "Account.NotFound",
        "This account does not exist.");

    public static readonly Error AlbumWithoutSuccessor = Error.Conflict(
        "Account.AlbumWithoutSuccessor",
        "The person owns albums without a successor: name one, or agree that the albums expire.");

    public static readonly Error DeletionAlreadyDecided = Error.Conflict(
        "Account.DeletionAlreadyDecided",
        "This deletion has already been decided.");

    public static readonly Error DeletionWindowOpen = Error.Conflict(
        "Account.DeletionWindowOpen",
        "The window for deleting this account has not ended yet.");

    public static readonly Error NoDeletionRequested = Error.NotFound(
        "Account.NoDeletionRequested",
        "Nobody asked for this account to be deleted.");

    public static readonly Error ProviderUnavailable = Error.NotFound(
        "Account.ProviderUnavailable",
        "This sign-in provider is not available.");
}

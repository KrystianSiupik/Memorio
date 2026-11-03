using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Memorio.Modules.Account.Infrastructure.Identity;

internal sealed class IdentityAccountRegistry(UserManager<AccountUser> users) : IAccountRegistry
{
    public async Task<Result<AccountId>> CreateWithPasswordAsync(string email, string password, AgeDeclaration declaration)
    {
        var user = new AccountUser
        {
            Id = Guid.CreateVersion7(),
            UserName = email,
            Email = email,
            AgeDeclaredAt = declaration.DeclaredAt,
        };

        try
        {
            var created = await users.CreateAsync(user, password);
            return created.Succeeded ? new AccountId(user.Id) : ErrorOf(created);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Two requests for one email at once: both pass Identity's check, the unique index decides.
            return AccountErrors.EmailTaken;
        }
    }

    public async Task<Result> RecordTermsAcceptanceAsync(AccountId accountId, string version, DateTimeOffset acceptedAt)
    {
        var user = await users.FindByIdAsync(accountId.Value.ToString());
        if (user is null)
        {
            return AccountErrors.NotFound;
        }

        if (user.AcceptedTermsVersion == version)
        {
            return Result.Success();
        }

        user.AcceptedTermsVersion = version;
        user.TermsAcceptedAt = acceptedAt;
        var updated = await users.UpdateAsync(user);
        return updated.Succeeded ? Result.Success() : ErrorOf(updated);
    }

    public static Error ErrorOf(IdentityResult result) =>
        result.Errors.Select(error => error.Code).ToList() switch
        {
            var codes when codes.Exists(code => code is "DuplicateEmail" or "DuplicateUserName") => AccountErrors.EmailTaken,
            var codes when codes.Exists(code => code.StartsWith("Password", StringComparison.Ordinal)) => AccountErrors.InvalidPassword,
            var codes when codes.Exists(code => code is "InvalidEmail" or "InvalidUserName") => AccountErrors.InvalidEmail,
            _ => Error.Failure("Account.CreateFailed", string.Join("; ", result.Errors.Select(error => error.Code))),
        };
}

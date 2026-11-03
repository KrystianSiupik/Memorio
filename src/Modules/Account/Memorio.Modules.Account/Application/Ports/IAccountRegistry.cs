using Memorio.Modules.Account.Domain.Accounts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Application.Ports;

/// <summary>Where account credentials are kept.</summary>
internal interface IAccountRegistry
{
    /// <summary>Fails with <see cref="AccountErrors.EmailTaken"/> when the email already has an account.</summary>
    Task<Result<AccountId>> CreateWithPasswordAsync(string email, string password, AgeDeclaration declaration);
}

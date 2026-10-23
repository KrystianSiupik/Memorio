using Microsoft.AspNetCore.Identity;

namespace Memorio.Modules.Account.Infrastructure.Identity;

/// <summary>Credentials of an account (ASP.NET Core Identity): password and/or Apple and Google sign-ins.</summary>
internal sealed class AccountUser : IdentityUser<Guid>
{
    /// <summary>When the person declared being at least 16 (<see cref="Domain.Accounts.AgeDeclaration"/>).</summary>
    public DateTimeOffset AgeDeclaredAt { get; set; }
}

using Memorio.Modules.Account.Infrastructure.Identity;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Account.Infrastructure.Persistence;

/// <summary>
/// The module's tables in its own schema (ADR 0004): Identity users, logins and tokens, and the Data
/// Protection keys that protect access and refresh tokens (shared by every API instance, kept across restarts).
/// </summary>
internal sealed class AccountDbContext(DbContextOptions<AccountDbContext> options)
    : IdentityUserContext<AccountUser, Guid>(options), IDataProtectionKeyContext
{
    public const string Schema = "account";

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Schema);
    }
}

using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
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

    public DbSet<AccountDeletion> AccountDeletions => Set<AccountDeletion>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Schema);
        builder.Entity<AccountUser>().Property(user => user.AcceptedTermsVersion).HasMaxLength(20);

        // Kept after the account is gone: only the id and dates, proof of what happened to the window.
        builder.Entity<AccountDeletion>(deletion =>
        {
            deletion.ToTable("account_deletions");
            deletion.HasKey(item => item.Id);
            deletion.Property(item => item.Id).HasColumnName("account_id").HasConversion(id => id.Value, value => new AccountId(value));
            deletion.Property(item => item.State).HasColumnName("state").HasConversion<string>().HasMaxLength(20).IsConcurrencyToken();
            deletion.Property(item => item.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            deletion.Property(item => item.RequestedAt).HasColumnName("requested_at");
            deletion.Property(item => item.WindowEndsAt).HasColumnName("window_ends_at");
            deletion.Property(item => item.ConsentsToAlbumExpiry).HasColumnName("consents_to_album_expiry");
            deletion.Ignore(item => item.DomainEvents);
        });
    }
}

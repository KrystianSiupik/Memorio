using Memorio.Modules.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Notifications.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004).</summary>
internal sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public const string Schema = "notifications";

    public DbSet<SentNotification> SentNotifications => Set<SentNotification>();

    public DbSet<BlockedAddress> BlockedAddresses => Set<BlockedAddress>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<SentNotification>(sent =>
        {
            sent.ToTable("sent_notifications");
            sent.HasKey(item => item.Subject);
            sent.Property(item => item.Subject).HasColumnName("subject").HasMaxLength(128);
            sent.Property(item => item.Sequence).HasColumnName("sequence");
            sent.Property(item => item.SentAt).HasColumnName("sent_at");
        });

        builder.Entity<BlockedAddress>(blocked =>
        {
            blocked.ToTable("blocked_addresses");
            blocked.HasKey(item => item.AddressHash);
            blocked.Property(item => item.AddressHash).HasColumnName("address_hash").HasMaxLength(64);
            blocked.Property(item => item.BlockedAt).HasColumnName("blocked_at");
        });
    }
}

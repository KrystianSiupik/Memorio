using Memorio.Modules.Access.Domain.Audiences;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Access.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004). Column names in snake_case: queries read them with Dapper (D-126).</summary>
internal sealed class AccessDbContext(DbContextOptions<AccessDbContext> options) : DbContext(options)
{
    public const string Schema = "access";

    public DbSet<AlbumAudience> Audiences => Set<AlbumAudience>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<LegacyGrant>(grant =>
        {
            grant.ToTable("legacy_grants");
            grant.HasKey(item => item.OwnerAccountId);
            grant.Property(item => item.OwnerAccountId).HasColumnName("owner_account_id").ValueGeneratedNever();
            grant.Property(item => item.ContactHash).HasColumnName("contact_hash").HasMaxLength(64);
            grant.Property(item => item.GrantedAt).HasColumnName("granted_at");
            grant.HasIndex(item => item.ContactHash).HasDatabaseName("ix_legacy_grants_contact_hash");
        });

        builder.Entity<AlbumAudience>(audience =>
        {
            audience.ToTable("album_audiences");
            audience.HasKey(item => item.Id);
            audience.Property(item => item.Id).HasColumnName("album_id").ValueGeneratedNever();
            audience.Property(item => item.LinkToken).HasColumnName("link_token").HasMaxLength(64);
            audience.Property(item => item.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            audience.Ignore(item => item.DomainEvents);
            audience.HasIndex(item => item.LinkToken).HasDatabaseName("ux_album_audiences_link_token").IsUnique().HasFilter("link_token IS NOT NULL");
            audience.OwnsMany(item => item.Viewers, viewer =>
            {
                viewer.ToTable("album_viewers");
                viewer.WithOwner().HasForeignKey("album_id");
                viewer.HasKey("album_id", nameof(AlbumViewer.AccountId));
                viewer.Property(item => item.AccountId).HasColumnName("account_id").ValueGeneratedNever();
                viewer.Property(item => item.EnteredAt).HasColumnName("entered_at");
                viewer.HasIndex(item => item.AccountId).HasDatabaseName("ix_album_viewers_account_id");
            });
            audience.Navigation(item => item.Viewers).UsePropertyAccessMode(PropertyAccessMode.Field);
            audience.OwnsMany(item => item.Blocks, block =>
            {
                block.ToTable("album_blocks");
                block.WithOwner().HasForeignKey("album_id");
                block.HasKey("album_id", nameof(AlbumBlock.AccountId));
                block.Property(item => item.AccountId).HasColumnName("account_id").ValueGeneratedNever();
                block.Property(item => item.BlockedAt).HasColumnName("blocked_at");
                block.HasIndex(item => item.AccountId).HasDatabaseName("ix_album_blocks_account_id");
            });
            audience.Navigation(item => item.Blocks).UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}

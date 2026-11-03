using Memorio.Modules.Album.Domain.Albums;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Album.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004). Column names in snake_case: queries read them with Dapper (D-126).</summary>
internal sealed class AlbumDbContext(DbContextOptions<AlbumDbContext> options) : DbContext(options)
{
    public const string Schema = "album";

    public DbSet<SharedAlbum> Albums => Set<SharedAlbum>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<SharedAlbum>(album =>
        {
            album.ToTable("albums");
            album.HasKey(item => item.Id);
            album.Property(item => item.Id).HasColumnName("id").HasConversion(id => id.Value, value => new AlbumId(value));
            album.Property(item => item.OwnerAccountId).HasColumnName("owner_account_id");
            album.Property(item => item.SuccessorAccountId).HasColumnName("successor_account_id");
            album.Ignore(item => item.DomainEvents);
            album.HasIndex(item => item.OwnerAccountId).HasDatabaseName("ix_albums_owner_account_id");
            album.OwnsMany(item => item.Participants, participant =>
            {
                participant.ToTable("album_participants");
                participant.WithOwner().HasForeignKey("album_id");
                participant.HasKey("album_id", nameof(AlbumParticipant.AccountId));
                participant.Property(item => item.AccountId).HasColumnName("account_id");
                participant.HasIndex(item => item.AccountId).HasDatabaseName("ix_album_participants_account_id");
            });
            album.Navigation(item => item.Participants).UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}

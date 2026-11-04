using Memorio.Modules.Album.Domain.Albums;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Album.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004). Column names in snake_case: queries read them with Dapper (D-126).</summary>
internal sealed class AlbumDbContext(DbContextOptions<AlbumDbContext> options) : DbContext(options)
{
    public const string Schema = "album";

    public DbSet<SharedAlbum> Albums => Set<SharedAlbum>();

    public DbSet<AlbumMemory> AlbumMemories => Set<AlbumMemory>();

    public DbSet<Entrustment> Entrustments => Set<Entrustment>();

    public DbSet<KeptContribution> KeptContributions => Set<KeptContribution>();

    public DbSet<AppliedAccountEvent> AppliedAccountEvents => Set<AppliedAccountEvent>();

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
            album.Property(item => item.Name).HasColumnName("name").HasMaxLength(SharedAlbum.MaximumNameLength).HasDefaultValue("");
            album.Property(item => item.CreatedAt).HasColumnName("created_at").HasDefaultValue(DateTimeOffset.UnixEpoch);
            album.Property(item => item.Sequence).HasColumnName("sequence").HasDefaultValue(1).IsConcurrencyToken();
            album.Ignore(item => item.DomainEvents);
            album.HasIndex(item => item.OwnerAccountId).HasDatabaseName("ix_albums_owner_account_id");
            album.OwnsMany(item => item.Participants, participant =>
            {
                participant.ToTable("album_participants");
                participant.WithOwner().HasForeignKey("album_id");
                participant.HasKey("album_id", nameof(AlbumParticipant.AccountId));
                participant.Property(item => item.AccountId).HasColumnName("account_id").ValueGeneratedNever();
                participant.Property(item => item.JoinedAt).HasColumnName("joined_at").HasDefaultValue(DateTimeOffset.UnixEpoch);
                participant.HasIndex(item => item.AccountId).HasDatabaseName("ix_album_participants_account_id");
            });
            album.Navigation(item => item.Participants).UsePropertyAccessMode(PropertyAccessMode.Field);
            album.OwnsMany(item => item.Invitations, invitation =>
            {
                invitation.ToTable("album_invitations");
                invitation.WithOwner().HasForeignKey("album_id");
                invitation.HasKey(item => item.Id);
                invitation.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
                invitation.Property(item => item.Email).HasColumnName("email").HasMaxLength(InvitationEmail.MaximumLength);
                invitation.Property(item => item.InvitedByAccountId).HasColumnName("invited_by_account_id");
                invitation.Property(item => item.InvitedAt).HasColumnName("invited_at");
                invitation.Property(item => item.State).HasColumnName("state").HasConversion<string>().HasMaxLength(16);
                invitation.Property(item => item.ReminderDueAt).HasColumnName("reminder_due_at");
                invitation.HasIndex(item => item.Email).HasDatabaseName("ix_album_invitations_email");
                invitation.HasIndex("album_id", nameof(AlbumInvitation.Email))
                    .HasDatabaseName("ux_album_invitations_pending")
                    .IsUnique()
                    .HasFilter("state = 'Pending'");
            });
            album.Navigation(item => item.Invitations).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<AlbumMemory>(memory =>
        {
            memory.ToTable("album_memories");
            memory.HasKey(item => new { item.AlbumId, item.MemoryId });
            memory.Property(item => item.AlbumId).HasColumnName("album_id").HasConversion(id => id.Value, value => new AlbumId(value));
            memory.Property(item => item.MemoryId).HasColumnName("memory_id");
            memory.Property(item => item.AuthorAccountId).HasColumnName("author_account_id");
            memory.Property(item => item.AddedAt).HasColumnName("added_at");
            memory.Property(item => item.Position).HasColumnName("position");
            memory.HasIndex(item => item.AuthorAccountId).HasDatabaseName("ix_album_memories_author_account_id");
            memory.HasIndex(item => item.MemoryId).HasDatabaseName("ix_album_memories_memory_id");
            memory.OwnsMany(item => item.Tags, tag =>
            {
                tag.ToTable("album_memory_tags");
                tag.WithOwner().HasForeignKey("album_id", "memory_id");
                tag.HasKey("album_id", "memory_id", nameof(MemoryTag.AccountId));
                tag.Property(item => item.AccountId).HasColumnName("account_id").ValueGeneratedNever();
                tag.Property(item => item.TaggedByAccountId).HasColumnName("tagged_by_account_id");
                tag.Property(item => item.TaggedAt).HasColumnName("tagged_at");
            });
            memory.Navigation(item => item.Tags).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<Entrustment>(entrustment =>
        {
            entrustment.ToTable("album_entrustments");
            entrustment.HasKey(item => new { item.AlbumId, item.AuthorAccountId });
            entrustment.Property(item => item.AlbumId).HasColumnName("album_id").HasConversion(id => id.Value, value => new AlbumId(value));
            entrustment.Property(item => item.AuthorAccountId).HasColumnName("author_account_id");
            entrustment.Property(item => item.CuratorAccountId).HasColumnName("curator_account_id");
            entrustment.Property(item => item.EntrustedAt).HasColumnName("entrusted_at");
            entrustment.HasIndex(item => item.CuratorAccountId).HasDatabaseName("ix_album_entrustments_curator_account_id");
        });

        builder.Entity<KeptContribution>(kept =>
        {
            kept.ToTable("album_kept_contributions");
            kept.HasKey(item => new { item.AlbumId, item.AuthorAccountId });
            kept.Property(item => item.AlbumId).HasColumnName("album_id").HasConversion(id => id.Value, value => new AlbumId(value));
            kept.Property(item => item.AuthorAccountId).HasColumnName("author_account_id");
            kept.Property(item => item.KeptAt).HasColumnName("kept_at");
            kept.Property(item => item.ChosenByAuthor).HasColumnName("chosen_by_author");
            kept.HasIndex(item => item.AuthorAccountId).HasDatabaseName("ix_album_kept_contributions_author_account_id");
        });

        builder.Entity<AppliedAccountEvent>(applied =>
        {
            applied.ToTable("applied_account_events");
            applied.HasKey(item => item.AccountId);
            applied.Property(item => item.AccountId).HasColumnName("account_id").ValueGeneratedNever();
            applied.Property(item => item.Sequence).HasColumnName("sequence");
        });
    }
}

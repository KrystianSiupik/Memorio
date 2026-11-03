using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004). Column names in snake_case: queries read them with Dapper (D-126).</summary>
internal sealed class ArchiveDbContext(DbContextOptions<ArchiveDbContext> options) : DbContext(options)
{
    public const string Schema = "archive";

    public DbSet<Memory> Memories => Set<Memory>();

    public DbSet<AppliedAccountEvent> AppliedAccountEvents => Set<AppliedAccountEvent>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<AppliedAccountEvent>(applied =>
        {
            applied.ToTable("applied_account_events");
            applied.HasKey(item => item.AccountId);
            applied.Property(item => item.AccountId).HasColumnName("account_id").ValueGeneratedNever();
            applied.Property(item => item.Sequence).HasColumnName("sequence");
        });

        builder.Entity<Memory>(memory =>
        {
            memory.ToTable("memories");
            memory.HasKey(item => item.Id);
            memory.Property(item => item.Id).HasColumnName("id").HasConversion(id => id.Value, value => new MemoryId(value));
            memory.Property(item => item.AuthorAccountId).HasColumnName("author_account_id");
            memory.Property(item => item.CreatedAt).HasColumnName("created_at");
            memory.Property(item => item.PlaceReference).HasColumnName("place_reference").HasMaxLength(100).HasDefaultValue("");
            memory.Property(item => item.PlaceName).HasColumnName("place_name").HasMaxLength(500).HasDefaultValue("");
            memory.Property(item => item.Latitude).HasColumnName("latitude").HasDefaultValue(0d);
            memory.Property(item => item.Longitude).HasColumnName("longitude").HasDefaultValue(0d);
            memory.Property(item => item.DatePrecision).HasColumnName("date_precision").HasConversion<string>().HasMaxLength(10).HasDefaultValue(DatePrecision.Day).HasSentinel((DatePrecision)(-1));
            memory.Property(item => item.DateFrom).HasColumnName("date_from").HasDefaultValue(new DateOnly(2000, 1, 1));
            memory.Property(item => item.DateTo).HasColumnName("date_to").HasDefaultValue(new DateOnly(2000, 1, 1));
            memory.Property(item => item.Description).HasColumnName("description").HasMaxLength(Memory.MaximumDescriptionLength);
            memory.Property(item => item.DescriptionAiModel).HasColumnName("description_ai_model").HasMaxLength(200);
            memory.Property(item => item.DescriptionAiPromptVersion).HasColumnName("description_ai_prompt_version").HasMaxLength(200);
            memory.Ignore(item => item.DomainEvents);
            memory.HasIndex(item => item.AuthorAccountId).HasDatabaseName("ix_memories_author_account_id");
            memory.OwnsMany(item => item.Photos, photo =>
            {
                photo.ToTable("photos");
                photo.WithOwner().HasForeignKey("memory_id");
                photo.HasKey(item => item.Id);
                photo.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
                photo.Property(item => item.StorageKey).HasColumnName("storage_key").HasMaxLength(512);
                photo.Property(item => item.SizeBytes).HasColumnName("size_bytes").HasDefaultValue(0L);
                photo.HasIndex("memory_id").HasDatabaseName("ix_photos_memory_id");
            });
            memory.Navigation(item => item.Photos).UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}

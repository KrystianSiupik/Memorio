using Memorio.Modules.AiNarration.Domain.Consents;
using Memorio.Modules.AiNarration.Domain.Drafts;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004).</summary>
internal sealed class AiNarrationDbContext(DbContextOptions<AiNarrationDbContext> options) : DbContext(options)
{
    public const string Schema = "ai_narration";

    public DbSet<MemoryDraft> MemoryDrafts => Set<MemoryDraft>();

    public DbSet<AiConsent> AiConsents => Set<AiConsent>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<MemoryDraft>(draft =>
        {
            draft.ToTable("memory_drafts");
            draft.HasKey(item => item.Id);
            draft.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
            draft.Property(item => item.AccountId).HasColumnName("account_id");
            draft.Property(item => item.Text).HasColumnName("text");
            draft.Property(item => item.Model).HasColumnName("model").HasMaxLength(200);
            draft.Property(item => item.PromptVersion).HasColumnName("prompt_version").HasMaxLength(200);
            draft.Property(item => item.CreatedAt).HasColumnName("created_at");
        });

        builder.Entity<AiConsent>(consent =>
        {
            consent.ToTable("ai_consents");
            consent.HasKey(item => item.Id);
            consent.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
            consent.Property(item => item.AccountId).HasColumnName("account_id");
            consent.Property(item => item.AlbumId).HasColumnName("album_id");
            consent.Property(item => item.IsGiven).HasColumnName("is_given");
            consent.Property(item => item.ChangedAt).HasColumnName("changed_at");
            consent.Property(item => item.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            consent.Ignore(item => item.DomainEvents);
            consent.HasIndex(item => new { item.AccountId, item.AlbumId }).HasDatabaseName("ux_ai_consents_account_album").IsUnique();
            consent.HasIndex(item => new { item.AlbumId, item.IsGiven }).HasDatabaseName("ix_ai_consents_album");
        });
    }
}

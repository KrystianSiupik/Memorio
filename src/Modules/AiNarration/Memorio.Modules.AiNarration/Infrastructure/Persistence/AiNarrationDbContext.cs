using Memorio.Modules.AiNarration.Domain.Drafts;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004).</summary>
internal sealed class AiNarrationDbContext(DbContextOptions<AiNarrationDbContext> options) : DbContext(options)
{
    public const string Schema = "ai_narration";

    public DbSet<MemoryDraft> MemoryDrafts => Set<MemoryDraft>();

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
    }
}

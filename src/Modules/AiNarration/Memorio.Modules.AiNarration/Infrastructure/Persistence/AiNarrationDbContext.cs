using System.Text.Json;
using Memorio.Modules.AiNarration.Domain.Consents;
using Memorio.Modules.AiNarration.Domain.Drafts;
using Memorio.Modules.AiNarration.Domain.Summaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004).</summary>
internal sealed class AiNarrationDbContext(DbContextOptions<AiNarrationDbContext> options) : DbContext(options)
{
    public const string Schema = "ai_narration";

    public DbSet<MemoryDraft> MemoryDrafts => Set<MemoryDraft>();

    public DbSet<AiConsent> AiConsents => Set<AiConsent>();

    public DbSet<PeriodSummary> PeriodSummaries => Set<PeriodSummary>();

    private static Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<List<T>, string> Json<T>() =>
        new(items => JsonSerializer.Serialize(items, JsonSerializerOptions.Web), json => JsonSerializer.Deserialize<List<T>>(json, JsonSerializerOptions.Web) ?? new List<T>());

    private static ValueComparer<List<T>> Comparer<T>() =>
        new((left, right) => JsonSerializer.Serialize(left, JsonSerializerOptions.Web) == JsonSerializer.Serialize(right, JsonSerializerOptions.Web),
            items => JsonSerializer.Serialize(items, JsonSerializerOptions.Web).GetHashCode(StringComparison.Ordinal),
            items => JsonSerializer.Deserialize<List<T>>(JsonSerializer.Serialize(items, JsonSerializerOptions.Web), JsonSerializerOptions.Web)!);

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

        builder.Entity<PeriodSummary>(summary =>
        {
            summary.ToTable("period_summaries");
            summary.HasKey(item => item.Id);
            summary.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
            summary.Property(item => item.AccountId).HasColumnName("account_id");
            summary.Property(item => item.AlbumId).HasColumnName("album_id");
            summary.Property(item => item.From).HasColumnName("period_from");
            summary.Property(item => item.To).HasColumnName("period_to");
            summary.Property(item => item.Model).HasColumnName("model").HasMaxLength(200);
            summary.Property(item => item.PromptVersion).HasColumnName("prompt_version").HasMaxLength(200);
            summary.Property(item => item.GeneratedAt).HasColumnName("generated_at");
            summary.Property(item => item.MaterialsReadAt).HasColumnName("materials_read_at");
            summary.Property(item => item.LeftOutMemories).HasColumnName("left_out_memories");
            summary.Property(item => item.LeftOutPeople).HasColumnName("left_out_people");
            summary.Property(item => item.DroppedSentences).HasColumnName("dropped_sentences");
            summary.Property(item => item.State).HasColumnName("state").HasConversion<string>().HasMaxLength(20);
            summary.Property(item => item.SavedAt).HasColumnName("saved_at");
            summary.Property(item => item.AddedToAlbumAt).HasColumnName("added_to_album_at");
            summary.Property(item => item.InvalidatedAt).HasColumnName("invalidated_at");
            summary.Property(item => item.InvalidReason).HasColumnName("invalid_reason").HasMaxLength(40);
            summary.Property(item => item.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            summary.Property<List<SummarySentence>>("_sentences").HasColumnName("sentences").HasColumnType("jsonb")
                .HasConversion(Json<SummarySentence>(), Comparer<SummarySentence>());
            summary.Property<List<SummarySource>>("_sources").HasColumnName("sources").HasColumnType("jsonb")
                .HasConversion(Json<SummarySource>(), Comparer<SummarySource>());
            summary.Ignore(item => item.Sentences);
            summary.Ignore(item => item.Sources);
            summary.Ignore(item => item.Text);
            summary.Ignore(item => item.DomainEvents);
            summary.HasIndex(item => new { item.AlbumId, item.AddedToAlbumAt }).HasDatabaseName("ix_period_summaries_album");
            summary.HasIndex(item => item.AccountId).HasDatabaseName("ix_period_summaries_account");
        });
    }
}

using Memorio.Modules.Provenance.Domain.Artifacts;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Provenance.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004), in snake_case: the policies look sources up with Dapper (D-126).</summary>
internal sealed class ProvenanceDbContext(DbContextOptions<ProvenanceDbContext> options) : DbContext(options)
{
    public const string Schema = "provenance";

    public DbSet<Artifact> Artifacts => Set<Artifact>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<Artifact>(artifact =>
        {
            artifact.ToTable("artifacts");
            artifact.HasKey(item => item.Id);
            artifact.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
            artifact.Property(item => item.Kind).HasColumnName("kind").HasMaxLength(Artifact.MaxKindLength);
            artifact.Property(item => item.OwnerAccountId).HasColumnName("owner_account_id");
            artifact.Property(item => item.AlbumId).HasColumnName("album_id");
            artifact.Property(item => item.CreatedAt).HasColumnName("created_at");
            artifact.Property(item => item.State).HasColumnName("state").HasConversion<string>().HasMaxLength(20);
            artifact.Property(item => item.InvalidatedAt).HasColumnName("invalidated_at");
            artifact.Property(item => item.Reason).HasColumnName("reason").HasConversion<string>().HasMaxLength(40);
            artifact.Property(item => item.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            artifact.Ignore(item => item.DomainEvents);
            artifact.HasIndex(item => new { item.AlbumId, item.OwnerAccountId }).HasDatabaseName("ix_artifacts_album_owner");

            artifact.OwnsMany(item => item.Sources, source =>
            {
                source.ToTable("artifact_sources");
                source.WithOwner().HasForeignKey("artifact_id");
                source.Property<Guid>("artifact_id");
                source.Property(item => item.MemoryId).HasColumnName("memory_id");
                source.Property(item => item.AuthorAccountId).HasColumnName("author_account_id");
                source.Property(item => item.AlbumId).HasColumnName("album_id");
                source.HasKey("artifact_id", nameof(ArtifactSource.MemoryId));
                source.HasIndex(item => item.MemoryId).HasDatabaseName("ix_artifact_sources_memory");
                source.HasIndex(item => new { item.AlbumId, item.AuthorAccountId }).HasDatabaseName("ix_artifact_sources_album_author");
            });
            artifact.Navigation(item => item.Sources).HasField("_sources").UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}

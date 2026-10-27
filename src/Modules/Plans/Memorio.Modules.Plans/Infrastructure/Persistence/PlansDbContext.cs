using Memorio.Modules.Plans.Domain.Entitlements;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Plans.Infrastructure.Persistence;

/// <summary>
/// The module's tables in its own schema (ADR 0004): the entitlement ledger (D-078). Column names in
/// snake_case: deploy/plans/*.sql read and write the ledger too.
/// </summary>
internal sealed class PlansDbContext(DbContextOptions<PlansDbContext> options) : DbContext(options)
{
    public const string Schema = "plans";

    public DbSet<EntitlementEntry> EntitlementEntries => Set<EntitlementEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<EntitlementEntry>(entry =>
        {
            entry.ToTable("entitlement_entries", table => table.HasCheckConstraint(
                "ck_entitlement_entries_manual_note",
                $"source <> '{EntitlementSources.Manual}' OR length(btrim(note)) > 0"));
            entry.HasKey(line => new { line.Source, line.SourceReference });
            entry.Ignore(line => line.Resource);
            entry.Property(line => line.Source).HasColumnName("source").HasMaxLength(100);
            entry.Property(line => line.SourceReference).HasColumnName("source_reference").HasMaxLength(200);
            entry.Property(line => line.ResourceKind).HasColumnName("resource_kind").HasConversion<string>().HasMaxLength(20);
            entry.Property(line => line.ResourceId).HasColumnName("resource_id");
            entry.Property(line => line.ValidUntil).HasColumnName("valid_until");
            entry.Property(line => line.OccurredAt).HasColumnName("occurred_at");
            entry.Property(line => line.RecordedAt).HasColumnName("recorded_at");
            entry.Property(line => line.Note).HasColumnName("note").HasMaxLength(500);
            entry.HasIndex(line => new { line.ResourceKind, line.ResourceId }).HasDatabaseName("ix_entitlement_entries_resource");
        });
    }
}

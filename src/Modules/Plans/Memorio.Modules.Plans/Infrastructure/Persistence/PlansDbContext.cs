using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Plans.Infrastructure.Persistence;

/// <summary>
/// The module's tables in its own schema (ADR 0004): the entitlement ledger (D-078) and the plan cycles (AG8). Column
/// names in snake_case: deploy/plans/*.sql read and write the ledger too, and queries read with Dapper (D-126).
/// </summary>
internal sealed class PlansDbContext(DbContextOptions<PlansDbContext> options) : DbContext(options)
{
    public const string Schema = "plans";

    public DbSet<EntitlementEntry> EntitlementEntries => Set<EntitlementEntry>();

    public DbSet<PlanCycle> PlanCycles => Set<PlanCycle>();

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

        builder.Entity<PlanCycle>(cycle =>
        {
            cycle.ToTable("plan_cycles");
            cycle.Ignore(item => item.Id);
            cycle.Ignore(item => item.DomainEvents);
            cycle.Ignore(item => item.IsHeld);
            cycle.HasKey(item => new { item.ResourceKind, item.ResourceId });
            cycle.Property(item => item.ResourceKind).HasColumnName("resource_kind").HasConversion<string>().HasMaxLength(20);
            cycle.Property(item => item.ResourceId).HasColumnName("resource_id");
            cycle.Property(item => item.Mode).HasColumnName("mode").HasConversion<string>().HasMaxLength(20);
            cycle.Property(item => item.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            cycle.Property(item => item.EntitledUntil).HasColumnName("entitled_until");
            cycle.Property(item => item.ReadOnlySince).HasColumnName("read_only_since");
            cycle.Property(item => item.TrimAt).HasColumnName("trim_at");
            cycle.Property(item => item.HeldSince).HasColumnName("held_since");
            cycle.Property(item => item.TrimmedAt).HasColumnName("trimmed_at");
            cycle.Property(item => item.LedgerCheckedAt).HasColumnName("ledger_checked_at");
            cycle.Property(item => item.IsOwnerless).HasColumnName("is_ownerless");
            cycle.Property(item => item.WarningRound).HasColumnName("warning_round");
            cycle.PrimitiveCollection(item => item.WarningsDue).HasColumnName("warnings_due").HasField("_warningsDue").UsePropertyAccessMode(PropertyAccessMode.Field);
            cycle.PrimitiveCollection(item => item.WarningsDelivered).HasColumnName("warnings_delivered").HasField("_warningsDelivered").UsePropertyAccessMode(PropertyAccessMode.Field);
            cycle.PrimitiveCollection(item => item.KeptMemoryIds).HasColumnName("kept_memory_ids").HasField("_keptMemoryIds").UsePropertyAccessMode(PropertyAccessMode.Field);
            cycle.HasIndex(item => new { item.Mode, item.EntitledUntil }).HasDatabaseName("ix_plan_cycles_mode_entitled_until");
            cycle.HasIndex(item => new { item.Mode, item.TrimAt }).HasDatabaseName("ix_plan_cycles_mode_trim_at");
        });
    }
}

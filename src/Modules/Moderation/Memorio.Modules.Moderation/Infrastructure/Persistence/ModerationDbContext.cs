using Memorio.Modules.Moderation.Domain.Reports;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Moderation.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004). Column names in snake_case: queries read them with Dapper (D-126), and deploy/moderation reads and writes them.</summary>
internal sealed class ModerationDbContext(DbContextOptions<ModerationDbContext> options) : DbContext(options)
{
    public const string Schema = "moderation";

    public DbSet<Report> Reports => Set<Report>();

    public DbSet<OperatorAction> Actions => Set<OperatorAction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<Report>(item =>
        {
            item.ToTable("reports", table =>
            {
                table.HasCheckConstraint("ck_reports_reporter", "reporter_account_id IS NOT NULL OR reporter_email IS NOT NULL");
                table.HasCheckConstraint("ck_reports_target", "target_id IS NOT NULL OR location IS NOT NULL");
            });
            item.HasKey(found => found.Id);
            item.Property(found => found.Id).HasColumnName("id").HasConversion(id => id.Value, value => new ReportId(value)).ValueGeneratedNever();
            item.Property(found => found.ReporterAccountId).HasColumnName("reporter_account_id");
            item.Property(found => found.ReporterEmail).HasColumnName("reporter_email").HasMaxLength(320);
            item.Property(found => found.Target).HasColumnName("target").HasConversion<string>().HasMaxLength(20);
            item.Property(found => found.TargetId).HasColumnName("target_id");
            item.Property(found => found.Location).HasColumnName("location").HasMaxLength(Report.MaximumLocationLength);
            item.Property(found => found.AuthorAccountId).HasColumnName("author_account_id");
            item.Property(found => found.Reason).HasColumnName("reason").HasConversion<string>().HasMaxLength(30);
            item.Property(found => found.Category).HasColumnName("category").HasConversion<string>().HasMaxLength(20);
            item.Property(found => found.Explanation).HasColumnName("explanation").HasMaxLength(Report.MaximumExplanationLength);
            item.Property(found => found.State).HasColumnName("state").HasConversion<string>().HasMaxLength(20);
            item.Property(found => found.ReportedAt).HasColumnName("reported_at");
            item.Property(found => found.DueBy).HasColumnName("due_by");
            item.Property(found => found.Hidden).HasColumnName("hidden");
            item.Property(found => found.Verdict).HasColumnName("verdict").HasConversion<string>().HasMaxLength(20);
            item.Property(found => found.Statement).HasColumnName("statement").HasMaxLength(Report.MaximumExplanationLength);
            item.Property(found => found.DecidedBy).HasColumnName("decided_by").HasMaxLength(200);
            item.Property(found => found.DecidedAt).HasColumnName("decided_at");
            item.Property(found => found.AppealUntil).HasColumnName("appeal_until");
            item.Property(found => found.AppealText).HasColumnName("appeal_text").HasMaxLength(Report.MaximumExplanationLength);
            item.Property(found => found.AppealedAt).HasColumnName("appealed_at");
            item.Property(found => found.AppealReversed).HasColumnName("appeal_reversed");
            item.Property(found => found.AppealStatement).HasColumnName("appeal_statement").HasMaxLength(Report.MaximumExplanationLength);
            item.Property(found => found.AppealDecidedAt).HasColumnName("appeal_decided_at");
            item.Property(found => found.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            item.Property(found => found.ReporterKey).HasColumnName("reporter_key").HasMaxLength(320);
            item.Property(found => found.TargetKey).HasColumnName("target_key").HasMaxLength(Report.MaximumLocationLength);
            item.Ignore(found => found.DomainEvents);
            item.HasIndex(found => found.ReporterAccountId).HasDatabaseName("ix_reports_reporter");
            item.HasIndex(found => found.AuthorAccountId).HasDatabaseName("ix_reports_author");
            // One open report of a person about a thing (S2:C2): a second at the same moment loses and reads the first.
            item.HasIndex(found => new { found.ReporterKey, found.TargetKey }).HasDatabaseName("ux_reports_open").IsUnique().HasFilter("state = 'Open'");
            item.HasIndex(found => found.DueBy).HasDatabaseName("ix_reports_queue").HasFilter("state IN ('Open', 'Appealed')");
        });

        builder.Entity<OperatorAction>(item =>
        {
            item.ToTable("operator_actions", table =>
            {
                table.HasCheckConstraint("ck_operator_actions_decided_by", "length(btrim(decided_by)) > 0");
                table.HasCheckConstraint("ck_operator_actions_statement", "length(btrim(statement)) > 0");
            });
            item.HasKey(found => found.Id);
            item.Property(found => found.Id).HasColumnName("id").ValueGeneratedNever();
            item.Property(found => found.Kind).HasColumnName("kind").HasMaxLength(30);
            item.Property(found => found.ReportId).HasColumnName("report_id");
            item.Property(found => found.Outcome).HasColumnName("outcome").HasMaxLength(20);
            item.Property(found => found.Statement).HasColumnName("statement").HasMaxLength(Report.MaximumExplanationLength);
            item.Property(found => found.DecidedBy).HasColumnName("decided_by").HasMaxLength(200);
            item.Property(found => found.DecidedAt).HasColumnName("decided_at");
            item.Property(found => found.AppliedAt).HasColumnName("applied_at");
            item.Property(found => found.Result).HasColumnName("result").HasMaxLength(100);
            item.HasIndex(found => found.DecidedAt).HasDatabaseName("ix_operator_actions_pending").HasFilter("applied_at IS NULL");
        });
    }
}

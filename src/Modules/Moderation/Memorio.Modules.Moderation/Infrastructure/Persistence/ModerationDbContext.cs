using Memorio.Modules.Moderation.Domain.Orders;
using Memorio.Modules.Moderation.Domain.Reports;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Moderation.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004). Column names in snake_case: queries read them with Dapper (D-126), and deploy/moderation reads and writes them.</summary>
internal sealed class ModerationDbContext(DbContextOptions<ModerationDbContext> options) : DbContext(options)
{
    public const string Schema = "moderation";

    public DbSet<Report> Reports => Set<Report>();

    public DbSet<OperatorAction> Actions => Set<OperatorAction>();

    public DbSet<AuthorityOrder> Orders => Set<AuthorityOrder>();

    public DbSet<IncomingOrder> IncomingOrders => Set<IncomingOrder>();

    public DbSet<OrderAction> OrderActions => Set<OrderAction>();

    public DbSet<AccessLogEntry> AccessLog => Set<AccessLogEntry>();

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

        builder.Entity<AuthorityOrder>(item =>
        {
            item.ToTable("orders");
            item.HasKey(found => found.Id);
            item.Property(found => found.Id).HasColumnName("id").HasConversion(id => id.Value, value => new OrderId(value)).ValueGeneratedNever();
            item.Property(found => found.Kind).HasColumnName("kind").HasConversion<string>().HasMaxLength(20);
            item.Property(found => found.Authority).HasColumnName("authority").HasMaxLength(AuthorityOrder.MaximumTextLength);
            item.Property(found => found.Reference).HasColumnName("reference").HasMaxLength(AuthorityOrder.MaximumTextLength);
            item.Property(found => found.ResourceKind).HasColumnName("resource_kind").HasConversion<string>().HasMaxLength(20);
            item.Property(found => found.ResourceId).HasColumnName("resource_id");
            item.Property(found => found.AccountId).HasColumnName("account_id");
            item.Property(found => found.Confidential).HasColumnName("confidential");
            item.Property(found => found.ReceivedBy).HasColumnName("received_by").HasMaxLength(200);
            item.Property(found => found.ReceivedAt).HasColumnName("received_at");
            item.Property(found => found.HiddenAt).HasColumnName("hidden_at");
            item.Property(found => found.SecuredAt).HasColumnName("secured_at");
            item.Property(found => found.DisclosedAt).HasColumnName("disclosed_at");
            item.Property(found => found.ClosedAt).HasColumnName("closed_at");
            item.Property(found => found.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            item.Ignore(found => found.DomainEvents);
            item.HasIndex(found => found.AccountId).HasDatabaseName("ix_orders_held_account").HasFilter("secured_at IS NOT NULL AND closed_at IS NULL");
        });

        builder.Entity<IncomingOrder>(item =>
        {
            item.ToTable("incoming_orders", table => table.HasCheckConstraint("ck_incoming_orders_received_by", "length(btrim(received_by)) > 0"));
            item.HasKey(found => found.Id);
            item.Property(found => found.Id).HasColumnName("id").ValueGeneratedNever();
            item.Property(found => found.Kind).HasColumnName("kind").HasMaxLength(20);
            item.Property(found => found.Authority).HasColumnName("authority").HasMaxLength(AuthorityOrder.MaximumTextLength);
            item.Property(found => found.Reference).HasColumnName("reference").HasMaxLength(AuthorityOrder.MaximumTextLength);
            item.Property(found => found.ResourceKind).HasColumnName("resource_kind").HasMaxLength(20);
            item.Property(found => found.ResourceId).HasColumnName("resource_id");
            item.Property(found => found.AccountId).HasColumnName("account_id");
            item.Property(found => found.Confidential).HasColumnName("confidential");
            item.Property(found => found.ReceivedBy).HasColumnName("received_by").HasMaxLength(200);
            item.Property(found => found.ReceivedAt).HasColumnName("received_at");
            item.Property(found => found.AppliedAt).HasColumnName("applied_at");
            item.Property(found => found.Result).HasColumnName("result").HasMaxLength(100);
            item.HasIndex(found => found.ReceivedAt).HasDatabaseName("ix_incoming_orders_pending").HasFilter("applied_at IS NULL");
        });

        builder.Entity<OrderAction>(item =>
        {
            item.ToTable("order_actions", table => table.HasCheckConstraint("ck_order_actions_decided_by", "length(btrim(decided_by)) > 0"));
            item.HasKey(found => found.Id);
            item.Property(found => found.Id).HasColumnName("id").ValueGeneratedNever();
            item.Property(found => found.OrderId).HasColumnName("order_id");
            item.Property(found => found.Kind).HasColumnName("kind").HasMaxLength(20);
            item.Property(found => found.Details).HasColumnName("details").HasMaxLength(AuthorityOrder.MaximumTextLength);
            item.Property(found => found.DecidedBy).HasColumnName("decided_by").HasMaxLength(200);
            item.Property(found => found.DecidedAt).HasColumnName("decided_at");
            item.Property(found => found.AppliedAt).HasColumnName("applied_at");
            item.Property(found => found.Result).HasColumnName("result").HasMaxLength(100);
            item.HasIndex(found => found.DecidedAt).HasDatabaseName("ix_order_actions_pending").HasFilter("applied_at IS NULL");
        });

        builder.Entity<AccessLogEntry>(item =>
        {
            item.ToTable("access_log", table => table.HasCheckConstraint("ck_access_log_by", "length(btrim(by)) > 0"));
            item.HasKey(found => found.Id);
            item.Property(found => found.Id).HasColumnName("id").ValueGeneratedNever();
            item.Property(found => found.At).HasColumnName("at");
            item.Property(found => found.By).HasColumnName("by").HasMaxLength(200);
            item.Property(found => found.Kind).HasColumnName("kind").HasMaxLength(20);
            item.Property(found => found.SubjectId).HasColumnName("subject_id");
            item.Property(found => found.ConcernsId).HasColumnName("concerns_id");
            item.Property(found => found.What).HasColumnName("what").HasMaxLength(AuthorityOrder.MaximumTextLength);
            item.Property(found => found.Basis).HasColumnName("basis").HasMaxLength(AuthorityOrder.MaximumTextLength);
            item.HasIndex(found => found.SubjectId).HasDatabaseName("ix_access_log_subject");
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

using Memorio.Modules.Succession.Domain.Cases;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Succession.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004). Column names in snake_case: queries read them with Dapper (D-126), and deploy/succession reads and writes them.</summary>
internal sealed class SuccessionDbContext(DbContextOptions<SuccessionDbContext> options) : DbContext(options)
{
    public const string Schema = "succession";

    public DbSet<TakeoverCase> Cases => Set<TakeoverCase>();

    public DbSet<LegacyContact> Contacts => Set<LegacyContact>();

    public DbSet<OperatorDecision> Decisions => Set<OperatorDecision>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<TakeoverCase>(item =>
        {
            item.ToTable("takeover_cases");
            item.HasKey(found => found.Id);
            item.Property(found => found.Id).HasColumnName("id").HasConversion(id => id.Value, value => new TakeoverCaseId(value)).ValueGeneratedNever();
            item.Property(found => found.AlbumId).HasColumnName("album_id");
            item.Property(found => found.OwnerAccountId).HasColumnName("owner_account_id");
            item.Property(found => found.ReportedByAccountId).HasColumnName("reported_by_account_id");
            item.Property(found => found.CertificateKey).HasColumnName("certificate_key").HasMaxLength(512);
            item.Property(found => found.State).HasColumnName("state").HasConversion<string>().HasMaxLength(20);
            item.Property(found => found.SuccessorAccountId).HasColumnName("successor_account_id");
            item.Property(found => found.ReportedAt).HasColumnName("reported_at");
            item.Property(found => found.AnswerBy).HasColumnName("answer_by");
            item.Property(found => found.ClosedAt).HasColumnName("closed_at");
            item.Property(found => found.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            item.Ignore(found => found.DomainEvents);
            item.HasIndex(found => found.AlbumId).HasDatabaseName("ux_takeover_cases_open_album").IsUnique().HasFilter("state IN ('Reported', 'Offered')");
            item.HasIndex(found => found.SuccessorAccountId).HasDatabaseName("ix_takeover_cases_successor").HasFilter("state = 'Offered'");
        });

        builder.Entity<LegacyContact>(item =>
        {
            item.ToTable("legacy_contacts");
            item.HasKey(found => found.Id);
            item.Property(found => found.Id).HasColumnName("owner_account_id").ValueGeneratedNever();
            item.Property(found => found.Email).HasColumnName("email").HasMaxLength(320);
            item.Property(found => found.SharedAt).HasColumnName("shared_at");
            item.Property(found => found.Sequence).HasColumnName("sequence").IsConcurrencyToken();
            item.Ignore(found => found.DomainEvents);
        });

        builder.Entity<OperatorDecision>(item =>
        {
            item.ToTable("operator_decisions", table => table.HasCheckConstraint("ck_operator_decisions_decided_by", "length(btrim(decided_by)) > 0"));
            item.HasKey(found => found.Id);
            item.Property(found => found.Id).HasColumnName("id").ValueGeneratedNever();
            item.Property(found => found.CaseId).HasColumnName("case_id");
            item.Property(found => found.Confirmed).HasColumnName("confirmed");
            item.Property(found => found.DecidedBy).HasColumnName("decided_by").HasMaxLength(200);
            item.Property(found => found.DecidedAt).HasColumnName("decided_at");
            item.Property(found => found.AppliedAt).HasColumnName("applied_at");
            item.Property(found => found.Outcome).HasColumnName("outcome").HasMaxLength(100);
            item.HasIndex(found => found.DecidedAt).HasDatabaseName("ix_operator_decisions_pending").HasFilter("applied_at IS NULL");
        });
    }
}

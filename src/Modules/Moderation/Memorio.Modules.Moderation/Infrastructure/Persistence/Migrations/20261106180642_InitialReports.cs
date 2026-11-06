using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Moderation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "moderation");

            migrationBuilder.CreateTable(
                name: "operator_actions",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    report_id = table.Column<Guid>(type: "uuid", nullable: false),
                    outcome = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    statement = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    decided_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    decided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    result = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operator_actions", x => x.id);
                    table.CheckConstraint("ck_operator_actions_decided_by", "length(btrim(decided_by)) > 0");
                    table.CheckConstraint("ck_operator_actions_statement", "length(btrim(statement)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "reports",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reporter_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reporter_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    target = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: true),
                    location = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    author_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reason = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    explanation = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    state = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    due_by = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    hidden = table.Column<bool>(type: "boolean", nullable: false),
                    verdict = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    statement = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    decided_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    decided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    appeal_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    appeal_text = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    appealed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    appeal_reversed = table.Column<bool>(type: "boolean", nullable: true),
                    appeal_statement = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    appeal_decided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    reporter_key = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    target_key = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.id);
                    table.CheckConstraint("ck_reports_reporter", "reporter_account_id IS NOT NULL OR reporter_email IS NOT NULL");
                    table.CheckConstraint("ck_reports_target", "target_id IS NOT NULL OR location IS NOT NULL");
                });

            migrationBuilder.CreateIndex(
                name: "ix_operator_actions_pending",
                schema: "moderation",
                table: "operator_actions",
                column: "decided_at",
                filter: "applied_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_reports_author",
                schema: "moderation",
                table: "reports",
                column: "author_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_queue",
                schema: "moderation",
                table: "reports",
                column: "due_by",
                filter: "state IN ('Open', 'Appealed')");

            migrationBuilder.CreateIndex(
                name: "ix_reports_reporter",
                schema: "moderation",
                table: "reports",
                column: "reporter_account_id");

            migrationBuilder.CreateIndex(
                name: "ux_reports_open",
                schema: "moderation",
                table: "reports",
                columns: new[] { "reporter_key", "target_key" },
                unique: true,
                filter: "state = 'Open'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operator_actions",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "reports",
                schema: "moderation");
        }
    }
}

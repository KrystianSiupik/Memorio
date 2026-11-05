using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Plans.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanCycles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plan_cycles",
                schema: "plans",
                columns: table => new
                {
                    resource_kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    entitled_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    read_only_since = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    trim_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    held_since = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    trimmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ledger_checked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_ownerless = table.Column<bool>(type: "boolean", nullable: false),
                    warning_round = table.Column<int>(type: "integer", nullable: false),
                    warnings_due = table.Column<int[]>(type: "integer[]", nullable: false),
                    warnings_delivered = table.Column<int[]>(type: "integer[]", nullable: false),
                    kept_memory_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_cycles", x => new { x.resource_kind, x.resource_id });
                });

            migrationBuilder.CreateIndex(
                name: "ix_plan_cycles_mode_entitled_until",
                schema: "plans",
                table: "plan_cycles",
                columns: new[] { "mode", "entitled_until" });

            migrationBuilder.CreateIndex(
                name: "ix_plan_cycles_mode_trim_at",
                schema: "plans",
                table: "plan_cycles",
                columns: new[] { "mode", "trim_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plan_cycles",
                schema: "plans");
        }
    }
}

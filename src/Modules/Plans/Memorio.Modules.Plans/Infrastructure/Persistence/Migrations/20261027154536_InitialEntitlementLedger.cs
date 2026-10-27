using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Plans.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialEntitlementLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "plans");

            migrationBuilder.CreateTable(
                name: "entitlement_entries",
                schema: "plans",
                columns: table => new
                {
                    source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    source_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    resource_kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valid_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entitlement_entries", x => new { x.source, x.source_reference });
                    table.CheckConstraint("ck_entitlement_entries_manual_note", "source <> 'manual' OR length(btrim(note)) > 0");
                });

            migrationBuilder.CreateIndex(
                name: "ix_entitlement_entries_resource",
                schema: "plans",
                table: "entitlement_entries",
                columns: new[] { "resource_kind", "resource_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entitlement_entries",
                schema: "plans");
        }
    }
}

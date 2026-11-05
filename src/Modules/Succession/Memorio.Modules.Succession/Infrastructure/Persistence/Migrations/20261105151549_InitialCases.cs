using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Succession.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "succession");

            migrationBuilder.CreateTable(
                name: "legacy_contacts",
                schema: "succession",
                columns: table => new
                {
                    owner_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    shared_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_legacy_contacts", x => x.owner_account_id);
                });

            migrationBuilder.CreateTable(
                name: "operator_decisions",
                schema: "succession",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    decided_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    decided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    outcome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operator_decisions", x => x.id);
                    table.CheckConstraint("ck_operator_decisions_decided_by", "length(btrim(decided_by)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "takeover_cases",
                schema: "succession",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reported_by_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    certificate_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    state = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    successor_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    answer_by = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_takeover_cases", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_operator_decisions_pending",
                schema: "succession",
                table: "operator_decisions",
                column: "decided_at",
                filter: "applied_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_takeover_cases_successor",
                schema: "succession",
                table: "takeover_cases",
                column: "successor_account_id",
                filter: "state = 'Offered'");

            migrationBuilder.CreateIndex(
                name: "ux_takeover_cases_open_album",
                schema: "succession",
                table: "takeover_cases",
                column: "album_id",
                unique: true,
                filter: "state IN ('Reported', 'Offered')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "legacy_contacts",
                schema: "succession");

            migrationBuilder.DropTable(
                name: "operator_decisions",
                schema: "succession");

            migrationBuilder.DropTable(
                name: "takeover_cases",
                schema: "succession");
        }
    }
}

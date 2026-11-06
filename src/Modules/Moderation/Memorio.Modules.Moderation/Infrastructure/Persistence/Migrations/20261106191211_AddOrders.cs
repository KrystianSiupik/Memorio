using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Moderation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "access_log",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    concerns_id = table.Column<Guid>(type: "uuid", nullable: true),
                    what = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    basis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_log", x => x.id);
                    table.CheckConstraint("ck_access_log_by", "length(btrim(by)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "incoming_orders",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    authority = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    reference = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    resource_kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: true),
                    account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    confidential = table.Column<bool>(type: "boolean", nullable: false),
                    received_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    result = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incoming_orders", x => x.id);
                    table.CheckConstraint("ck_incoming_orders_received_by", "length(btrim(received_by)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "order_actions",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    decided_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    decided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    result = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_actions", x => x.id);
                    table.CheckConstraint("ck_order_actions_decided_by", "length(btrim(decided_by)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    authority = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    reference = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    resource_kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: true),
                    account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    confidential = table.Column<bool>(type: "boolean", nullable: false),
                    received_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    hidden_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    secured_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    disclosed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_access_log_subject",
                schema: "moderation",
                table: "access_log",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "ix_incoming_orders_pending",
                schema: "moderation",
                table: "incoming_orders",
                column: "received_at",
                filter: "applied_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_order_actions_pending",
                schema: "moderation",
                table: "order_actions",
                column: "decided_at",
                filter: "applied_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_orders_held_account",
                schema: "moderation",
                table: "orders",
                column: "account_id",
                filter: "secured_at IS NOT NULL AND closed_at IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_log",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "incoming_orders",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "order_actions",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "moderation");
        }
    }
}

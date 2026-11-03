using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Limits.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAiAllowance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ai_limit",
                schema: "limits",
                table: "account_pools",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ai_period",
                schema: "limits",
                table: "account_pools",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2000, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "ai_used",
                schema: "limits",
                table: "account_pools",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ai_operations",
                schema: "limits",
                columns: table => new
                {
                    operation_key = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consumed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_operations", x => x.operation_key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_operations",
                schema: "limits");

            migrationBuilder.DropColumn(
                name: "ai_limit",
                schema: "limits",
                table: "account_pools");

            migrationBuilder.DropColumn(
                name: "ai_period",
                schema: "limits",
                table: "account_pools");

            migrationBuilder.DropColumn(
                name: "ai_used",
                schema: "limits",
                table: "account_pools");
        }
    }
}

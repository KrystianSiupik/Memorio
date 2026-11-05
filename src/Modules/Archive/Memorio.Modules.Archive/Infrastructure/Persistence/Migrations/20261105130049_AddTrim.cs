using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Archive.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTrim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "trimmed_for",
                schema: "archive",
                table: "memories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "applied_plan_events",
                schema: "archive",
                columns: table => new
                {
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applied_plan_events", x => x.resource_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_memories_trimmed_for",
                schema: "archive",
                table: "memories",
                column: "trimmed_for",
                filter: "trimmed_for IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applied_plan_events",
                schema: "archive");

            migrationBuilder.DropIndex(
                name: "ix_memories_trimmed_for",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "trimmed_for",
                schema: "archive",
                table: "memories");
        }
    }
}

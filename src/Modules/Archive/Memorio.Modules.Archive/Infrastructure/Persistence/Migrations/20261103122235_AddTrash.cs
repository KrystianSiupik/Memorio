using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Archive.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTrash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sequence",
                schema: "archive",
                table: "memories",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "state",
                schema: "archive",
                table: "memories",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.AddColumn<string>(
                name: "trash_reason",
                schema: "archive",
                table: "memories",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "trashed_at",
                schema: "archive",
                table: "memories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "window_ends_at",
                schema: "archive",
                table: "memories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_memories_trash_window_ends_at",
                schema: "archive",
                table: "memories",
                column: "window_ends_at",
                filter: "state = 'Trashed'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_memories_trash_window_ends_at",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "sequence",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "state",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "trash_reason",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "trashed_at",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "window_ends_at",
                schema: "archive",
                table: "memories");
        }
    }
}

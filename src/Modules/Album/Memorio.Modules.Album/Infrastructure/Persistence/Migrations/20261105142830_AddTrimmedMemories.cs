using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Album.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTrimmedMemories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "applied_plan_events",
                schema: "album",
                columns: table => new
                {
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applied_plan_events", x => x.album_id);
                });

            migrationBuilder.CreateTable(
                name: "trimmed_memories",
                schema: "album",
                columns: table => new
                {
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    memory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trimmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    has_left = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trimmed_memories", x => new { x.album_id, x.memory_id });
                });

            migrationBuilder.CreateIndex(
                name: "ix_trimmed_memories_author_account_id",
                schema: "album",
                table: "trimmed_memories",
                column: "author_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_trimmed_memories_memory_id",
                schema: "album",
                table: "trimmed_memories",
                column: "memory_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applied_plan_events",
                schema: "album");

            migrationBuilder.DropTable(
                name: "trimmed_memories",
                schema: "album");
        }
    }
}

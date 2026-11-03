using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Archive.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMemories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "archive");

            migrationBuilder.CreateTable(
                name: "memories",
                schema: "archive",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "photos",
                schema: "archive",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    storage_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    memory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK_photos_memories_memory_id",
                        column: x => x.memory_id,
                        principalSchema: "archive",
                        principalTable: "memories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_memories_author_account_id",
                schema: "archive",
                table: "memories",
                column: "author_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_photos_memory_id",
                schema: "archive",
                table: "photos",
                column: "memory_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "photos",
                schema: "archive");

            migrationBuilder.DropTable(
                name: "memories",
                schema: "archive");
        }
    }
}

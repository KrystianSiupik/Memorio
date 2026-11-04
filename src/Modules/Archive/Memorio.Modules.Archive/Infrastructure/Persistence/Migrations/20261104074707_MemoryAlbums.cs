using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Archive.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MemoryAlbums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "album_id",
                schema: "archive",
                table: "photos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "memory_albums",
                schema: "archive",
                columns: table => new
                {
                    memory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memory_albums", x => new { x.memory_id, x.album_id });
                });

            migrationBuilder.CreateIndex(
                name: "ix_memory_albums_album_id",
                schema: "archive",
                table: "memory_albums",
                column: "album_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "memory_albums",
                schema: "archive");

            migrationBuilder.DropColumn(
                name: "album_id",
                schema: "archive",
                table: "photos");
        }
    }
}

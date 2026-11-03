using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Album.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAlbums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "album");

            migrationBuilder.CreateTable(
                name: "albums",
                schema: "album",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    successor_account_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_albums", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "album_participants",
                schema: "album",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_participants", x => new { x.album_id, x.account_id });
                    table.ForeignKey(
                        name: "FK_album_participants_albums_album_id",
                        column: x => x.album_id,
                        principalSchema: "album",
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_album_participants_account_id",
                schema: "album",
                table: "album_participants",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_albums_owner_account_id",
                schema: "album",
                table: "albums",
                column: "owner_account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "album_participants",
                schema: "album");

            migrationBuilder.DropTable(
                name: "albums",
                schema: "album");
        }
    }
}

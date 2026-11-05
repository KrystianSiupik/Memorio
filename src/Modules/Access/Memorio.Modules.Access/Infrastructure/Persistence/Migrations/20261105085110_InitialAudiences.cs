using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Access.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAudiences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "access");

            migrationBuilder.CreateTable(
                name: "album_audiences",
                schema: "access",
                columns: table => new
                {
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    link_token = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_audiences", x => x.album_id);
                });

            migrationBuilder.CreateTable(
                name: "album_blocks",
                schema: "access",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blocked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_blocks", x => new { x.album_id, x.account_id });
                    table.ForeignKey(
                        name: "FK_album_blocks_album_audiences_album_id",
                        column: x => x.album_id,
                        principalSchema: "access",
                        principalTable: "album_audiences",
                        principalColumn: "album_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_viewers",
                schema: "access",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_viewers", x => new { x.album_id, x.account_id });
                    table.ForeignKey(
                        name: "FK_album_viewers_album_audiences_album_id",
                        column: x => x.album_id,
                        principalSchema: "access",
                        principalTable: "album_audiences",
                        principalColumn: "album_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_album_audiences_link_token",
                schema: "access",
                table: "album_audiences",
                column: "link_token",
                unique: true,
                filter: "link_token IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_album_blocks_account_id",
                schema: "access",
                table: "album_blocks",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_viewers_account_id",
                schema: "access",
                table: "album_viewers",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "album_blocks",
                schema: "access");

            migrationBuilder.DropTable(
                name: "album_viewers",
                schema: "access");

            migrationBuilder.DropTable(
                name: "album_audiences",
                schema: "access");
        }
    }
}

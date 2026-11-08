using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AiConsents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_consents",
                schema: "ai_narration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_given = table.Column<bool>(type: "boolean", nullable: false),
                    changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_consents", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ai_consents_album",
                schema: "ai_narration",
                table: "ai_consents",
                columns: new[] { "album_id", "is_given" });

            migrationBuilder.CreateIndex(
                name: "ux_ai_consents_account_album",
                schema: "ai_narration",
                table: "ai_consents",
                columns: new[] { "account_id", "album_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_consents",
                schema: "ai_narration");
        }
    }
}

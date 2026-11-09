using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PeriodSummaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "period_summaries",
                schema: "ai_narration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_from = table.Column<DateOnly>(type: "date", nullable: false),
                    period_to = table.Column<DateOnly>(type: "date", nullable: false),
                    model = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    prompt_version = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    materials_read_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    left_out_memories = table.Column<int>(type: "integer", nullable: false),
                    left_out_people = table.Column<int>(type: "integer", nullable: false),
                    dropped_sentences = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    saved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    added_to_album_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    invalidated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    invalid_reason = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    sentences = table.Column<string>(type: "jsonb", nullable: false),
                    sources = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_period_summaries", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_period_summaries_account",
                schema: "ai_narration",
                table: "period_summaries",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_period_summaries_album",
                schema: "ai_narration",
                table: "period_summaries",
                columns: new[] { "album_id", "added_to_album_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "period_summaries",
                schema: "ai_narration");
        }
    }
}

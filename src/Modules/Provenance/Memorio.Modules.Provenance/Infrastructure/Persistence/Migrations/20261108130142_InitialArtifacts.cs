using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Provenance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialArtifacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "provenance");

            migrationBuilder.CreateTable(
                name: "artifacts",
                schema: "provenance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    owner_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    state = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    invalidated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    reason = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_artifacts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "artifact_sources",
                schema: "provenance",
                columns: table => new
                {
                    memory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    artifact_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_artifact_sources", x => new { x.artifact_id, x.memory_id });
                    table.ForeignKey(
                        name: "FK_artifact_sources_artifacts_artifact_id",
                        column: x => x.artifact_id,
                        principalSchema: "provenance",
                        principalTable: "artifacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_artifact_sources_album_author",
                schema: "provenance",
                table: "artifact_sources",
                columns: new[] { "album_id", "author_account_id" });

            migrationBuilder.CreateIndex(
                name: "ix_artifact_sources_memory",
                schema: "provenance",
                table: "artifact_sources",
                column: "memory_id");

            migrationBuilder.CreateIndex(
                name: "ix_artifacts_album_owner",
                schema: "provenance",
                table: "artifacts",
                columns: new[] { "album_id", "owner_account_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "artifact_sources",
                schema: "provenance");

            migrationBuilder.DropTable(
                name: "artifacts",
                schema: "provenance");
        }
    }
}

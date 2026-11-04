using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Album.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlbumsInvitationsContributions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "album",
                table: "albums",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "album",
                table: "albums",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sequence",
                schema: "album",
                table: "albums",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "joined_at",
                schema: "album",
                table: "album_participants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "album_entrustments",
                schema: "album",
                columns: table => new
                {
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    curator_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entrusted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_entrustments", x => new { x.album_id, x.author_account_id });
                });

            migrationBuilder.CreateTable(
                name: "album_invitations",
                schema: "album",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    invited_by_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invited_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    state = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reminder_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_invitations", x => x.id);
                    table.ForeignKey(
                        name: "FK_album_invitations_albums_album_id",
                        column: x => x.album_id,
                        principalSchema: "album",
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_kept_contributions",
                schema: "album",
                columns: table => new
                {
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kept_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    chosen_by_author = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_kept_contributions", x => new { x.album_id, x.author_account_id });
                });

            migrationBuilder.CreateTable(
                name: "album_memories",
                schema: "album",
                columns: table => new
                {
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    memory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    added_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_memories", x => new { x.album_id, x.memory_id });
                });

            migrationBuilder.CreateTable(
                name: "applied_account_events",
                schema: "album",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applied_account_events", x => x.account_id);
                });

            migrationBuilder.CreateTable(
                name: "album_memory_tags",
                schema: "album",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    memory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tagged_by_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tagged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_memory_tags", x => new { x.album_id, x.memory_id, x.account_id });
                    table.ForeignKey(
                        name: "FK_album_memory_tags_album_memories_album_id_memory_id",
                        columns: x => new { x.album_id, x.memory_id },
                        principalSchema: "album",
                        principalTable: "album_memories",
                        principalColumns: new[] { "album_id", "memory_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_album_entrustments_curator_account_id",
                schema: "album",
                table: "album_entrustments",
                column: "curator_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_invitations_email",
                schema: "album",
                table: "album_invitations",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ux_album_invitations_pending",
                schema: "album",
                table: "album_invitations",
                columns: new[] { "album_id", "email" },
                unique: true,
                filter: "state = 'Pending'");

            migrationBuilder.CreateIndex(
                name: "ix_album_kept_contributions_author_account_id",
                schema: "album",
                table: "album_kept_contributions",
                column: "author_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_memories_author_account_id",
                schema: "album",
                table: "album_memories",
                column: "author_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_memories_memory_id",
                schema: "album",
                table: "album_memories",
                column: "memory_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "album_entrustments",
                schema: "album");

            migrationBuilder.DropTable(
                name: "album_invitations",
                schema: "album");

            migrationBuilder.DropTable(
                name: "album_kept_contributions",
                schema: "album");

            migrationBuilder.DropTable(
                name: "album_memory_tags",
                schema: "album");

            migrationBuilder.DropTable(
                name: "applied_account_events",
                schema: "album");

            migrationBuilder.DropTable(
                name: "album_memories",
                schema: "album");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "album",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "name",
                schema: "album",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "sequence",
                schema: "album",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "joined_at",
                schema: "album",
                table: "album_participants");
        }
    }
}

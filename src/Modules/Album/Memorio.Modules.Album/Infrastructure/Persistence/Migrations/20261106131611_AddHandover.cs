using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Album.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHandover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "handover_keeps_contributions",
                schema: "album",
                table: "albums",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "handover_to_account_id",
                schema: "album",
                table: "albums",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "handover_until",
                schema: "album",
                table: "albums",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "handover_keeps_contributions",
                schema: "album",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "handover_to_account_id",
                schema: "album",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "handover_until",
                schema: "album",
                table: "albums");
        }
    }
}

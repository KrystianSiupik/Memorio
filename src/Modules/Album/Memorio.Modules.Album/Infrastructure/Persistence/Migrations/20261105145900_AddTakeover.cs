using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Album.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTakeover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ownerless_since",
                schema: "album",
                table: "albums",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "reserved_by_account_id",
                schema: "album",
                table: "albums",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "reserved_until",
                schema: "album",
                table: "albums",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ownerless_since",
                schema: "album",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "reserved_by_account_id",
                schema: "album",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "reserved_until",
                schema: "album",
                table: "albums");
        }
    }
}

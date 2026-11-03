using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Limits.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRelease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "released_at",
                schema: "limits",
                table: "pool_operations",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "released_at",
                schema: "limits",
                table: "pool_operations");
        }
    }
}

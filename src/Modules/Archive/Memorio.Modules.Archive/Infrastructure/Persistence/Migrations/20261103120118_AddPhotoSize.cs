using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Archive.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "size_bytes",
                schema: "archive",
                table: "photos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "size_bytes",
                schema: "archive",
                table: "photos");
        }
    }
}

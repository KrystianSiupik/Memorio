using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Plans.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLastAlbumSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "last_album_sequence",
                schema: "plans",
                table: "plan_cycles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_album_sequence",
                schema: "plans",
                table: "plan_cycles");
        }
    }
}

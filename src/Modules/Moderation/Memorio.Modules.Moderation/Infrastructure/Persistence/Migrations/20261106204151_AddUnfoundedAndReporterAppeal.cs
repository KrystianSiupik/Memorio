using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Moderation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnfoundedAndReporterAppeal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "appeal_by_reporter",
                schema: "moderation",
                table: "reports",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "unfounded",
                schema: "moderation",
                table: "reports",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_reports_unfounded",
                schema: "moderation",
                table: "reports",
                column: "reporter_key",
                filter: "unfounded");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_reports_unfounded",
                schema: "moderation",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "appeal_by_reporter",
                schema: "moderation",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "unfounded",
                schema: "moderation",
                table: "reports");
        }
    }
}

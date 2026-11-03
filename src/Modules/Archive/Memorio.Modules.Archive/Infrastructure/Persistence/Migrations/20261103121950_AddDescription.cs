using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Archive.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "archive",
                table: "memories",
                type: "character varying(20000)",
                maxLength: 20000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description_ai_model",
                schema: "archive",
                table: "memories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description_ai_prompt_version",
                schema: "archive",
                table: "memories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "description_ai_model",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "description_ai_prompt_version",
                schema: "archive",
                table: "memories");
        }
    }
}

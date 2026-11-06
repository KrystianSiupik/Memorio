using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Access.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHiddenScopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hidden_scopes",
                schema: "access",
                columns: table => new
                {
                    hold_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resource_kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    hidden = table.Column<bool>(type: "boolean", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hidden_scopes", x => x.hold_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_hidden_scopes_resource",
                schema: "access",
                table: "hidden_scopes",
                column: "resource_id",
                filter: "hidden");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hidden_scopes",
                schema: "access");
        }
    }
}

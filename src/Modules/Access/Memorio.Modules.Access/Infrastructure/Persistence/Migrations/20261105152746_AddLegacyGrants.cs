using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Access.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLegacyGrants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "legacy_grants",
                schema: "access",
                columns: table => new
                {
                    owner_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contact_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    granted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_legacy_grants", x => x.owner_account_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_legacy_grants_contact_hash",
                schema: "access",
                table: "legacy_grants",
                column: "contact_hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "legacy_grants",
                schema: "access");
        }
    }
}

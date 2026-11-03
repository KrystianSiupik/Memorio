using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Limits.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "limits");

            migrationBuilder.CreateTable(
                name: "account_pools",
                schema: "limits",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    used_bytes = table.Column<long>(type: "bigint", nullable: false),
                    limit_bytes = table.Column<long>(type: "bigint", nullable: false),
                    limit_sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_pools", x => x.account_id);
                });

            migrationBuilder.CreateTable(
                name: "pool_operations",
                schema: "limits",
                columns: table => new
                {
                    operation_key = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bytes = table.Column<long>(type: "bigint", nullable: false),
                    reserved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pool_operations", x => x.operation_key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_pools",
                schema: "limits");

            migrationBuilder.DropTable(
                name: "pool_operations",
                schema: "limits");
        }
    }
}

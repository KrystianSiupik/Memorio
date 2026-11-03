using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Account.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingDeletionWindowIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_account_deletions_pending_window_ends_at",
                schema: "account",
                table: "account_deletions",
                column: "window_ends_at",
                filter: "state = 'Pending'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_account_deletions_pending_window_ends_at",
                schema: "account",
                table: "account_deletions");
        }
    }
}

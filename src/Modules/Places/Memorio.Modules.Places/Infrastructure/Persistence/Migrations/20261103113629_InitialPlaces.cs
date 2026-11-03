using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Places.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPlaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "places");

            // Filled by the OpenStreetMap import, which replaces it (D-131); here so suggestions work without one.
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS places.osm_places (
                    osm_type char(1) NOT NULL,
                    osm_id bigint NOT NULL,
                    name text NOT NULL,
                    kind text NOT NULL,
                    tags jsonb NOT NULL,
                    geom geometry(Point, 4326) NOT NULL)
                """);

            migrationBuilder.CreateTable(
                name: "own_places",
                schema: "places",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_own_places", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_own_places_owner_account_id",
                schema: "places",
                table: "own_places",
                column: "owner_account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "own_places",
                schema: "places");
        }
    }
}

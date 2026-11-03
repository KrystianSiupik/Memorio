using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Modules.Archive.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMemoryPlaceAndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "date_from",
                schema: "archive",
                table: "memories",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2000, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "date_precision",
                schema: "archive",
                table: "memories",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Day");

            migrationBuilder.AddColumn<DateOnly>(
                name: "date_to",
                schema: "archive",
                table: "memories",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2000, 1, 1));

            migrationBuilder.AddColumn<double>(
                name: "latitude",
                schema: "archive",
                table: "memories",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                schema: "archive",
                table: "memories",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "place_name",
                schema: "archive",
                table: "memories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "place_reference",
                schema: "archive",
                table: "memories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_from",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "date_precision",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "date_to",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "latitude",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "longitude",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "place_name",
                schema: "archive",
                table: "memories");

            migrationBuilder.DropColumn(
                name: "place_reference",
                schema: "archive",
                table: "memories");
        }
    }
}

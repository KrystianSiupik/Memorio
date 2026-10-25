// Migrations for the SD18 rule tests: one per kind of change the rule must allow or refuse.
using Memorio.Modularity;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Memorio.ArchitectureTests.Fixtures.Migrations;

internal sealed class ExpandsOnly : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("photos", table => new { id = table.Column<Guid>() }, schema: "album");
        migrationBuilder.AddColumn<string>("caption", "photos", schema: "album", nullable: true);
        migrationBuilder.AddColumn<int>("views", "photos", schema: "album", nullable: false, defaultValue: 0);
        migrationBuilder.CreateIndex("ix_photos_caption", "photos", "caption", schema: "album");
        migrationBuilder.AlterColumn<string>("caption", "photos", schema: "album", maxLength: 500, nullable: true,
            oldClrType: typeof(string), oldMaxLength: 200, oldNullable: true);
        migrationBuilder.DropIndex("ix_photos_old", "photos", schema: "album");
    }

    // Down reverts a deployment by hand; the rule looks at Up only.
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("photos", schema: "album");
}

internal sealed class Contracts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("drafts", schema: "album");
        migrationBuilder.DropColumn("legacy", "photos", schema: "album");
        migrationBuilder.RenameColumn("title", "photos", "name", schema: "album");
        migrationBuilder.RenameTable("pics", schema: "album", newName: "pictures");
        migrationBuilder.AddColumn<Guid>("owner", "photos", schema: "album", nullable: false);
        migrationBuilder.AlterColumn<string>("caption", "photos", schema: "album", maxLength: 100, nullable: true,
            oldClrType: typeof(string), oldMaxLength: 200, oldNullable: true);
        migrationBuilder.AlterColumn<string>("place", "photos", schema: "album", nullable: false,
            oldClrType: typeof(string), oldNullable: true);
        migrationBuilder.AlterColumn<long>("size", "photos", schema: "album", oldClrType: typeof(int));
        migrationBuilder.Sql("UPDATE album.photos SET views = 0");
    }
}

[ContractMigration("Release 0.3 stopped reading album.photos.legacy.")]
internal sealed class MarkedContract : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn("legacy", "photos", schema: "album");
}

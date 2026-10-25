using Memorio.ArchitectureTests.Fixtures.Migrations;

namespace Memorio.ArchitectureTests;

/// <summary>The SD18 rule itself, on the fixture migrations, and on every migration of the modules.</summary>
public sealed class MigrationLintTests
{
    [Fact]
    public void Finds_the_migrations()
    {
        var migrations = MigrationLint.MigrationsIn(typeof(MigrationLintTests).Assembly.GetTypes())
            .Where(type => type.Namespace == typeof(ExpandsOnly).Namespace)
            .Select(type => type.Name)
            .Order(StringComparer.Ordinal);

        Assert.Equal(["Contracts", "ExpandsOnly", "MarkedContract"], migrations);
    }

    [Fact]
    public void Adding_tables_nullable_or_defaulted_columns_indexes_and_widening_is_expanding()
    {
        Assert.Empty(MigrationLint.Violations([typeof(ExpandsOnly)]));
    }

    [Fact]
    public void Removing_renaming_narrowing_required_columns_and_raw_SQL_are_refused()
    {
        Assert.Equal(
            [
                "Contracts: adds required column photos.owner without a default",
                "Contracts: drops column photos.legacy",
                "Contracts: drops table drafts",
                "Contracts: narrows column photos.caption",
                "Contracts: narrows column photos.place",
                "Contracts: narrows column photos.size",
                "Contracts: renames column photos.title",
                "Contracts: renames table pics",
                "Contracts: runs raw SQL",
            ],
            MigrationLint.Violations([typeof(Contracts)]));
    }

    [Fact]
    public void A_contract_migration_marked_with_its_reason_is_allowed()
    {
        Assert.Empty(MigrationLint.Violations([typeof(MarkedContract)]));
    }

    [Fact]
    public void SD18_every_module_migration_only_expands_unless_marked_as_contract()
    {
        var modules = Directory.GetFiles(AppContext.BaseDirectory, "Memorio.Modules.*.dll")
            .SelectMany(file => System.Reflection.Assembly.LoadFrom(file).GetTypes());
        var migrations = MigrationLint.MigrationsIn(modules);

        Assert.NotEmpty(migrations);
        var violations = MigrationLint.Violations(migrations);
        Assert.True(
            violations.Count == 0,
            "SD18: split into expand now and contract in a later release ([ContractMigration(reason)]): " + string.Join("; ", violations));
    }
}

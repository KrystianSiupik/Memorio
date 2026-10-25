using System.Reflection;
using Memorio.Modularity;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Memorio.ArchitectureTests;

/// <summary>
/// Fitness function SD18 (D-041): migrations only expand the schema. During a deployment and after a
/// rollback the previous version runs on the migrated database, so a migration must not remove, rename or
/// narrow anything that version uses, nor add a column it would have to fill. Such a change is split:
/// expand in one release, contract in a later one, marked <see cref="ContractMigrationAttribute"/>.
/// Raw SQL cannot be judged and needs the mark too.
/// </summary>
internal static class MigrationLint
{
    /// <summary>EF Core migrations (concrete <see cref="Migration"/> types) among <paramref name="types"/>.</summary>
    public static IReadOnlyList<Type> MigrationsIn(IEnumerable<Type> types) =>
        [.. types.Where(type => type is { IsClass: true, IsAbstract: false } && type.IsSubclassOf(typeof(Migration)))];

    /// <summary>"Migration: problem" for every contracting operation of an unmarked migration.</summary>
    public static IReadOnlyList<string> Violations(IEnumerable<Type> migrations) =>
        [
            .. migrations
                .Where(type => type.GetCustomAttribute<ContractMigrationAttribute>() is null)
                .SelectMany(type => ((Migration)Activator.CreateInstance(type)!).UpOperations
                    .Select(Problem)
                    .OfType<string>()
                    .Select(problem => $"{type.Name}: {problem}"))
                .Order(StringComparer.Ordinal),
        ];

    private static string? Problem(MigrationOperation operation) => operation switch
    {
        DropTableOperation drop => $"drops table {drop.Name}",
        DropColumnOperation drop => $"drops column {drop.Table}.{drop.Name}",
        DropSchemaOperation drop => $"drops schema {drop.Name}",
        DropSequenceOperation drop => $"drops sequence {drop.Name}",
        DropPrimaryKeyOperation drop => $"drops primary key {drop.Name}",
        RenameTableOperation rename => $"renames table {rename.Name}",
        RenameColumnOperation rename => $"renames column {rename.Table}.{rename.Name}",
        AddColumnOperation add when !add.IsNullable && add.DefaultValue is null
                                    && add.DefaultValueSql is null && add.ComputedColumnSql is null =>
            $"adds required column {add.Table}.{add.Name} without a default",
        AlterColumnOperation alter when Narrows(alter) => $"narrows column {alter.Table}.{alter.Name}",
        SqlOperation => "runs raw SQL",
        _ => null,
    };

    private static bool Narrows(AlterColumnOperation alter) =>
        (!alter.IsNullable && alter.OldColumn.IsNullable)
        || alter.ClrType != alter.OldColumn.ClrType
        || (alter.ColumnType is not null && alter.ColumnType != alter.OldColumn.ColumnType)
        || (alter.MaxLength is { } length && (alter.OldColumn.MaxLength is null || length < alter.OldColumn.MaxLength));
}

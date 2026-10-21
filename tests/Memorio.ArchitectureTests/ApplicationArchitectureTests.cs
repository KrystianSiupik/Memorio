using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

namespace Memorio.ArchitectureTests;

/// <summary>Fitness functions SD1 and SD2 (ADR 0001, 0002, 0019) on the real modules.</summary>
public sealed class ApplicationArchitectureTests
{
    private const string ModulesRoot = "Memorio.Modules";

    /// <summary>The 14 modules of the module map; PlaceProfiles and Publishing come after MVP (D-070).</summary>
    private static readonly string[] ExpectedModules =
    [
        "Access", "Account", "AiNarration", "Album", "Archive", "Limits", "Moderation",
        "Notifications", "PlaceProfiles", "Places", "Plans", "Provenance", "Publishing", "Succession",
    ];

    private static readonly string[] ModuleAssemblyFiles =
        Directory.GetFiles(AppContext.BaseDirectory, $"{ModulesRoot}.*.dll");

    private static readonly Architecture Application = new ArchLoader()
        .LoadAssemblies([.. ModuleAssemblyFiles.Select(System.Reflection.Assembly.LoadFrom)])
        .Build();

    public static TheoryData<string> RuleNames =>
        [.. ModuleRules.All(ModulesRoot, ExpectedModules, requirePositiveResults: false).Select(rule => rule.Name)];

    [Fact]
    public void Every_module_of_the_map_has_a_module_assembly_and_a_contracts_assembly()
    {
        var expectedFiles = ExpectedModules
            .SelectMany(module => new[] { $"{ModulesRoot}.{module}.dll", $"{ModulesRoot}.{module}.Contracts.dll" })
            .Order(StringComparer.Ordinal);
        var actualFiles = ModuleAssemblyFiles.Select(Path.GetFileName).Order(StringComparer.Ordinal);

        Assert.Equal(expectedFiles, actualFiles);
        Assert.Equal(ExpectedModules, ModuleRules.ModulesIn(Application, ModulesRoot));
    }

    // Most layers are still empty, so a rule may have nothing to check yet; ModuleRulesTests proves
    // on fixtures that each rule catches its violation.
    [Theory]
    [MemberData(nameof(RuleNames))]
    public void Modules_follow_the_rule(string ruleName)
    {
        var rule = ModuleRules.All(ModulesRoot, ExpectedModules, requirePositiveResults: false).Single(rule => rule.Name == ruleName);

        Assert.True(
            rule.Rule.HasNoViolations(Application),
            $"Architecture rule broken: {ruleName}");
    }
}

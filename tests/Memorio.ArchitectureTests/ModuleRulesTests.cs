using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Memorio.Modularity;

namespace Memorio.ArchitectureTests;

/// <summary>
/// The rules themselves: a correct layout passes, and each kind of violation is caught by exactly
/// the rule meant for it (fixtures in <c>Fixtures/</c>). Without this, a rule with a wrong pattern
/// would pass on the application forever.
/// </summary>
public sealed class ModuleRulesTests
{
    private const string FixturesRoot = "Memorio.ArchitectureTests.Fixtures";

    private static readonly Architecture Fixtures = new ArchLoader()
        .LoadAssemblies(typeof(ModuleRulesTests).Assembly, typeof(IModule).Assembly)
        .Build();

    [Fact]
    public void A_correct_layout_passes_every_rule_and_every_rule_checks_something()
    {
        var root = $"{FixturesRoot}.Valid.Modules";
        var modules = ModuleRules.ModulesIn(Fixtures, root);
        Assert.Equal(["Alpha", "Beta"], modules);

        // Positive results required: a rule that matches no type fails here.
        var failing = ModuleRules.All(root, modules, requirePositiveResults: true)
            .Where(rule => !rule.Rule.HasNoViolations(Fixtures))
            .Select(rule => rule.Name);

        Assert.Empty(failing);
    }

    [Theory]
    [InlineData("CrossModule", "SD1: Alpha uses other modules only through their Contracts")]
    [InlineData("ContractsExposeInside", "SD1: Contracts do not expose the inside of any module")]
    [InlineData("DomainUsesInfrastructure", "SD2: Domain does not depend on Application, Infrastructure or Api")]
    [InlineData("ApplicationUsesApi", "SD2: Application does not depend on Infrastructure or Api")]
    [InlineData("InfrastructureUsesApi", "SD2: Infrastructure does not depend on Api")]
    [InlineData("DomainUsesFramework", "SD2: Domain does not know frameworks (HTTP, EF Core, Wolverine, Quartz, DI)")]
    [InlineData("ApplicationUsesFramework", "SD2: Application does not know HTTP or persistence frameworks")]
    [InlineData("PublicInside", "Visibility: the only public types inside a module are its IModule entry point and its message handlers")]
    [InlineData("MessagingNotStaticHandler", "Visibility: public types in Infrastructure.Messaging are static *Handler classes")]
    public void A_violation_is_caught_by_its_rule_only(string fixture, string expectedRule)
    {
        var root = $"{FixturesRoot}.{fixture}.Modules";
        var rules = ModuleRules.All(root, ModuleRules.ModulesIn(Fixtures, root), requirePositiveResults: false);
        Assert.Contains(rules, rule => rule.Name == expectedRule);

        var failing = rules
            .Where(rule => !rule.Rule.HasNoViolations(Fixtures))
            .Select(rule => rule.Name);

        Assert.Equal([expectedRule], failing);
    }
}

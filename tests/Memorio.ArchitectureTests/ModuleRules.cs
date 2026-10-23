using System.Text.RegularExpressions;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using Memorio.Modularity;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Memorio.ArchitectureTests;

/// <summary>
/// Architecture rules for modules living under one root namespace ("Memorio.Modules" in the
/// application, a fixture namespace in the rule tests).
/// A module is the namespace <c>{root}.{Module}</c>; its public contract is <c>{root}.{Module}.Contracts</c>,
/// everything else is the module's inside, split into the Onion layers Domain, Application,
/// Infrastructure and Api (ADR 0001, ADR 0019).
/// </summary>
internal static class ModuleRules
{
    /// <summary>Frameworks the domain must not know (ADR 0019: no Wolverine, EF Core or HTTP).</summary>
    private const string FrameworksForbiddenInDomain =
        @"^(Microsoft\.AspNetCore|Microsoft\.EntityFrameworkCore|Microsoft\.Extensions|Npgsql|Wolverine|Quartz|System\.Net\.Http)(\..*)?$";

    /// <summary>Frameworks the application layer must not know: persistence and HTTP stay in Infrastructure and Api.</summary>
    private const string FrameworksForbiddenInApplication =
        @"^(Microsoft\.AspNetCore|Microsoft\.EntityFrameworkCore|Npgsql|System\.Net\.Http)(\..*)?$";

    /// <param name="requirePositiveResults">
    /// When true, a rule that matches no type fails (used on fixtures, where every rule must check something).
    /// </param>
    public static IReadOnlyList<NamedRule> All(string root, IEnumerable<string> modules, bool requirePositiveResults)
    {
        var r = Regex.Escape(root);
        var insideOfAnyModule = $@"^{r}\.\w+(\.(?!Contracts(\.|$))[\w.]+)?$";
        string Layer(string pattern) => $@"^{r}\.\w+\.({pattern})(\.[\w.]+)?$";

        NamedRule Rule(string name, TypesShouldConjunction rule) =>
            new(name, requirePositiveResults ? rule : rule.WithoutRequiringPositiveResults());

        var rules = new List<NamedRule>();

        foreach (var module in modules)
        {
            var m = Regex.Escape(module);
            rules.Add(Rule(
                $"SD1: {module} uses other modules only through their Contracts",
                Types().That().ResideInNamespaceMatching($@"^{r}\.{m}(\.(?!Contracts(\.|$))[\w.]+)?$")
                    .Should().NotDependOnAnyTypesThat()
                    .ResideInNamespaceMatching($@"^{r}\.(?!{m}(\.|$))\w+(\.(?!Contracts(\.|$))[\w.]+)?$")));
        }

        rules.Add(Rule(
            "SD1: Contracts do not expose the inside of any module",
            Types().That().ResideInNamespaceMatching($@"^{r}\.\w+\.Contracts(\.[\w.]+)?$")
                .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(insideOfAnyModule)));

        rules.Add(Rule(
            "SD2: Domain does not depend on Application, Infrastructure or Api",
            Types().That().ResideInNamespaceMatching(Layer("Domain"))
                .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(Layer("Application|Infrastructure|Api"))));

        rules.Add(Rule(
            "SD2: Application does not depend on Infrastructure or Api",
            Types().That().ResideInNamespaceMatching(Layer("Application"))
                .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(Layer("Infrastructure|Api"))));

        rules.Add(Rule(
            "SD2: Infrastructure does not depend on Api",
            Types().That().ResideInNamespaceMatching(Layer("Infrastructure"))
                .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(Layer("Api"))));

        rules.Add(Rule(
            "SD2: Domain does not know frameworks (HTTP, EF Core, Wolverine, Quartz, DI)",
            Types().That().ResideInNamespaceMatching(Layer("Domain"))
                .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(FrameworksForbiddenInDomain)));

        rules.Add(Rule(
            "SD2: Application does not know HTTP or persistence frameworks",
            Types().That().ResideInNamespaceMatching(Layer("Application"))
                .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(FrameworksForbiddenInApplication)));

        // Wolverine only runs public handlers (D-135): a module's message handlers are the one exception.
        var messaging = $@"^{r}\.\w+\.Infrastructure\.Messaging$";

        rules.Add(Rule(
            "Visibility: the only public types inside a module are its IModule entry point and its message handlers",
            Types().That().ResideInNamespaceMatching(insideOfAnyModule).And().ArePublic()
                .And().DoNotResideInNamespaceMatching(messaging)
                .Should().ImplementInterface(typeof(IModule))));

        rules.Add(Rule(
            "Visibility: public types in Infrastructure.Messaging are static *Handler classes",
            Types().That().ResideInNamespaceMatching(messaging).And().ArePublic()
                .Should().HaveNameEndingWith("Handler")
                .AndShould().FollowCustomCondition(
                    type => type is Class { IsAbstract: true, IsSealed: true },
                    "be static classes",
                    "is not a static class")));

        return rules;
    }

    /// <summary>Module names found under <paramref name="root"/>: the namespace segment right after it.</summary>
    public static IReadOnlyList<string> ModulesIn(Architecture architecture, string root) =>
        architecture.Types
            .Select(type => type.Namespace.FullName)
            .Where(ns => ns.StartsWith(root + ".", StringComparison.Ordinal))
            .Select(ns => ns[(root.Length + 1)..].Split('.')[0])
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToList();
}

internal sealed record NamedRule(string Name, IArchRule Rule)
{
    public override string ToString() => Name;
}

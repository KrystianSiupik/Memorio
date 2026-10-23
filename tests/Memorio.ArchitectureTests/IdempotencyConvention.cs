using System.Text.RegularExpressions;

namespace Memorio.ArchitectureTests;

/// <summary>
/// Fitness function SD6 (ADR 0006): delivery is at least once, so every message handler of a module
/// (a public static *Handler class in <c>{root}.{Module}.Infrastructure.Messaging</c>) has a test class
/// <c>{Handler}IdempotencyTests</c> in that module's test project, which delivers the same event twice
/// and checks that the state is the same as after one delivery.
/// </summary>
internal static partial class IdempotencyConvention
{
    public const string TestClassSuffix = "IdempotencyTests";

    public static IReadOnlyList<(string Module, Type Handler)> HandlersIn(IEnumerable<Type> types, string root)
    {
        var messaging = new Regex($@"^{Regex.Escape(root)}\.(?<module>\w+)\.Infrastructure\.Messaging$");
        return
        [
            .. types
                .Where(type => type is { IsPublic: true, IsAbstract: true, IsSealed: true }
                    && type.Name.EndsWith("Handler", StringComparison.Ordinal))
                .Select(type => (Match: messaging.Match(type.Namespace ?? ""), Type: type))
                .Where(handler => handler.Match.Success)
                .Select(handler => (handler.Match.Groups["module"].Value, handler.Type)),
        ];
    }

    /// <summary>Handlers ("Module.Handler") whose module has no idempotency test class for them.</summary>
    public static IReadOnlyList<string> HandlersWithoutTest(
        IEnumerable<(string Module, Type Handler)> handlers,
        Func<string, IEnumerable<string>> testClassesOfModule) =>
        [
            .. handlers
                .Where(handler => !testClassesOfModule(handler.Module).Contains(handler.Handler.Name + TestClassSuffix))
                .Select(handler => $"{handler.Module}.{handler.Handler.Name}")
                .Order(StringComparer.Ordinal),
        ];

    /// <summary>Idempotency test classes declared in the sources under <paramref name="directory"/>.</summary>
    public static IEnumerable<string> TestClassesIn(string directory) =>
        !Directory.Exists(directory)
            ? []
            : Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
                .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .SelectMany(file => TestClassDeclaration().Matches(File.ReadAllText(file)).Select(match => match.Groups[1].Value));

    [GeneratedRegex($@"\bclass\s+(\w+{TestClassSuffix})\b")]
    private static partial Regex TestClassDeclaration();
}

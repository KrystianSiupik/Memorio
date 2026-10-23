using System.Text;
using Memorio.Hosting;

namespace Memorio.Testing;

/// <summary>
/// Naming rule of ADR 0004 (D-128): every module in <see cref="ModuleCatalog"/> has the schema
/// snake_case(Name) and the role memorio_&lt;schema&gt;, e.g. AiNarration → ai_narration, memorio_ai_narration.
/// </summary>
public static class ModuleSchemas
{
    public static IReadOnlyList<string> All { get; } =
        [.. ModuleCatalog.All.Select(module => SchemaOf(module.Name)).Order(StringComparer.Ordinal)];

    public static string RoleOf(string schema) => "memorio_" + schema;

    public static string SchemaOf(string moduleName)
    {
        var schema = new StringBuilder();
        foreach (var character in moduleName)
        {
            if (char.IsUpper(character) && schema.Length > 0)
            {
                schema.Append('_');
            }

            schema.Append(char.ToLowerInvariant(character));
        }

        return schema.ToString();
    }
}

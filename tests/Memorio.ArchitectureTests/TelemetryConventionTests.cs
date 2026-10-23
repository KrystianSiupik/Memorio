using System.Reflection;
using Memorio.Hosting.Telemetry;

namespace Memorio.ArchitectureTests;

/// <summary>
/// Every meter of Memorio is exported (ADR 0014): a meter's name is a constant <c>MeterName</c> of the type
/// that creates it, starting with <see cref="MemorioTelemetry.MeterPrefix"/>, which the hosts export
/// without a list to keep up to date.
/// </summary>
public sealed class TelemetryConventionTests
{
    private static readonly IReadOnlyList<(string Type, string Name)> MeterNames =
    [
        .. Directory.GetFiles(AppContext.BaseDirectory, "Memorio.*.dll")
            .Where(file => !Path.GetFileName(file).Contains("Tests", StringComparison.Ordinal))
            .SelectMany(file => Assembly.LoadFrom(file).GetTypes())
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(field => field is { IsLiteral: true, Name: "MeterName" } && field.FieldType == typeof(string))
                .Select(field => (type.FullName!, (string)field.GetRawConstantValue()!))),
    ];

    [Fact]
    public void The_known_meters_are_found()
    {
        Assert.Superset(
            new HashSet<string>(["Memorio.Messaging", "Memorio.Scheduling", "Memorio.Account"]),
            MeterNames.Select(meter => meter.Name).ToHashSet());
    }

    [Fact]
    public void Every_meter_is_named_with_the_exported_prefix()
    {
        var unexported = MeterNames.Where(meter => !meter.Name.StartsWith(MemorioTelemetry.MeterPrefix, StringComparison.Ordinal));

        Assert.Empty(unexported);
    }
}

using Microsoft.Extensions.Options;

namespace Memorio.Api.Configuration;

/// <summary>
/// Configuration comes from environment variables only (RD1) and is validated at startup. Validation
/// messages name the variable, never its value: use <see cref="Variable"/> to build them.
/// </summary>
internal static partial class ConfigurationErrors
{
    /// <summary>The environment variable of a configuration key: "AppVersion:Minimum" → "AppVersion__Minimum".</summary>
    public static string Variable(string key) => key.Replace(":", "__", StringComparison.Ordinal);

    /// <summary>Logs every invalid variable once, without values, so the operator knows what to fix.</summary>
    public static void Log(ILogger logger, OptionsValidationException exception)
    {
        foreach (var failure in exception.Failures)
        {
            InvalidConfiguration(logger, failure);
        }
    }

    [LoggerMessage(Level = LogLevel.Critical, Message = "Invalid configuration: {Failure}")]
    private static partial void InvalidConfiguration(ILogger logger, string failure);
}

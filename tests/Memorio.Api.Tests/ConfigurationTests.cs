using Microsoft.Extensions.Options;

namespace Memorio.Api.Tests;

/// <summary>RD1: configuration from environment variables, validated at startup, errors without values.</summary>
public sealed class ConfigurationTests
{
    [Fact]
    public void The_required_settings_are_enough_to_start()
    {
        using var factory = new MemorioApiFactory();

        using var client = factory.CreateClient();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("secret-looking-value-1.x")]
    public void A_missing_or_invalid_variable_stops_the_host_and_names_the_variable_not_the_value(string? value)
    {
        using var factory = MemorioApiFactory.With("AppVersion:Minimum", value);

        var error = Assert.ThrowsAny<OptionsValidationException>(() => factory.CreateClient());

        var failure = Assert.Single(error.Failures);
        Assert.StartsWith("AppVersion__Minimum ", failure, StringComparison.Ordinal);
        if (!string.IsNullOrEmpty(value))
        {
            Assert.DoesNotContain(value, error.ToString(), StringComparison.Ordinal);
        }
    }
}

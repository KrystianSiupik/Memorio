using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;

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

    [Theory]
    [InlineData("Storage:Endpoint", null, "Storage__Endpoint is required and must be an http(s) address.")]
    [InlineData("Storage:Endpoint", "fsn1.example.com", "Storage__Endpoint is required and must be an http(s) address.")]
    [InlineData("Storage:PublicEndpoint", "not an address", "Storage__PublicEndpoint must be an http(s) address when set.")]
    [InlineData("Storage:Bucket", null, "Storage__Bucket is required.")]
    [InlineData("Storage:SecretKey", "", "Storage__SecretKey is required.")]
    public void An_invalid_storage_setting_stops_the_host_and_names_the_variable_not_the_value(string key, string? value, string expected)
    {
        using var factory = MemorioApiFactory.With(key, value);

        var error = Assert.ThrowsAny<OptionsValidationException>(() => factory.CreateClient());

        Assert.Equal(expected, Assert.Single(error.Failures));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void A_missing_messaging_connection_stops_the_host_and_names_the_variable(string? value)
    {
        using var factory = MemorioApiFactory.With("Messaging:ConnectionString", value);

        var error = Assert.ThrowsAny<OptionsValidationException>(() => factory.CreateClient());

        Assert.Equal("Messaging__ConnectionString is required.", Assert.Single(error.Failures));
    }

    [Theory]
    [InlineData("otel-collector:4318")]
    [InlineData("ftp://otel-collector")]
    public void An_invalid_telemetry_address_stops_the_host_and_names_the_variable(string value)
    {
        using var factory = MemorioApiFactory.With("Telemetry:OtlpEndpoint", value);

        var error = Assert.ThrowsAny<OptionsValidationException>(() => factory.CreateClient());

        Assert.Equal("Telemetry__OtlpEndpoint must be an http(s) address when set.", Assert.Single(error.Failures));
    }

    [Fact]
    public void Telemetry_is_exported_only_when_its_address_is_set()
    {
        using var without = new MemorioApiFactory();
        using var with = MemorioApiFactory.With("Telemetry:OtlpEndpoint", "http://localhost:4318");

        Assert.Null(without.Services.GetService<MeterProvider>());
        Assert.NotNull(with.Services.GetService<MeterProvider>());
    }
}

using Amazon.Runtime;
using Amazon.S3;
using Memorio.Hosting.Configuration;
using Memorio.Hosting.Health;
using Memorio.SharedKernel.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Hosting.Storage;

/// <summary>Registers <see cref="IObjectStorage"/> on the configured bucket (ADR 0005) for both hosts.</summary>
public static class ObjectStorage
{
    public const string HealthCheckName = "object-storage";

    public static IServiceCollection AddMemorioObjectStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<StorageOptions>()
            .Bind(configuration.GetSection(StorageOptions.Section))
            .Validate(options => IsAddress(options.Endpoint), Invalid(nameof(StorageOptions.Endpoint), "is required and must be an http(s) address"))
            .Validate(
                options => string.IsNullOrEmpty(options.PublicEndpoint) || IsAddress(options.PublicEndpoint),
                Invalid(nameof(StorageOptions.PublicEndpoint), "must be an http(s) address when set"))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Region), Invalid(nameof(StorageOptions.Region), "is required"))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Bucket), Invalid(nameof(StorageOptions.Bucket), "is required"))
            .Validate(options => !string.IsNullOrWhiteSpace(options.AccessKey), Invalid(nameof(StorageOptions.AccessKey), "is required"))
            .Validate(options => !string.IsNullOrWhiteSpace(options.SecretKey), Invalid(nameof(StorageOptions.SecretKey), "is required"))
            .ValidateOnStart();

        // Invalid values fail the validation above at startup; the placeholders only let the host be built.
        var options = configuration.GetSection(StorageOptions.Section).Get<StorageOptions>() ?? new StorageOptions();
        var endpoint = IsAddress(options.Endpoint) ? options.Endpoint! : "http://unconfigured";
        var publicEndpoint = IsAddress(options.PublicEndpoint) ? options.PublicEndpoint! : endpoint;
        var region = string.IsNullOrWhiteSpace(options.Region) ? "unconfigured" : options.Region;
        var bucket = string.IsNullOrWhiteSpace(options.Bucket) ? "unconfigured" : options.Bucket;
        var credentials = new BasicAWSCredentials(
            string.IsNullOrWhiteSpace(options.AccessKey) ? "unconfigured" : options.AccessKey,
            string.IsNullOrWhiteSpace(options.SecretKey) ? "unconfigured" : options.SecretKey);

        var client = new AmazonS3Client(credentials, ClientConfig(endpoint, region));
        var signer = new AmazonS3Client(credentials, ClientConfig(publicEndpoint, region));
        var publicProtocol = new Uri(publicEndpoint).Scheme == Uri.UriSchemeHttps ? Protocol.HTTPS : Protocol.HTTP;

        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<IObjectStorage>(provider =>
            new S3ObjectStorage(client, signer, bucket, publicProtocol, provider.GetRequiredService<TimeProvider>()));
        services.AddHealthChecks().Add(new HealthCheckRegistration(
            HealthCheckName,
            _ => new ObjectStorageHealthCheck(client, bucket),
            failureStatus: null,
            tags: [HealthEndpoints.ReadyTag]));
        return services;
    }

    /// <summary>Path-style addressing and checksums only when required: what S3-compatible services expect.</summary>
    internal static AmazonS3Config ClientConfig(string endpoint, string region) => new()
    {
        ServiceURL = endpoint,
        AuthenticationRegion = region,
        ForcePathStyle = true,
        RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
        ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
    };

    private static bool IsAddress(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);

    private static string Invalid(string key, string rule) =>
        $"{ConfigurationErrors.Variable($"{StorageOptions.Section}:{key}")} {rule}.";
}

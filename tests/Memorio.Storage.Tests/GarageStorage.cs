using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Memorio.Hosting.Storage;
using Memorio.SharedKernel.Storage;

namespace Memorio.Storage.Tests;

/// <summary>
/// The local S3-compatible server of compose.yaml (Garage, tools/storage/garage.toml) with its default
/// bucket, prepared like every environment's bucket (tools/storage/lifecycle.json). Needs Docker.
/// </summary>
public sealed class GarageStorage : IAsyncLifetime
{
    public const string Bucket = "memorio-media";
    public const string Region = "garage";

    private const string AccessKey = "GK6d656d6f72696f2d6c6f63616c6b6579";
    private const string SecretKey = "6d656d6f72696f2d6c6f63616c2d6761726167652d7365637265742d6b657921";
    private const int S3Port = 3900;

    private readonly IContainer _container = new ContainerBuilder("dxflrs/garage:v2.4.1")
        .WithResourceMapping(new FileInfo(Path.Combine(RepositoryRoot(), "tools", "storage", "garage.toml")), new FileInfo("/etc/garage.toml"))
        .WithEnvironment("GARAGE_DEFAULT_ACCESS_KEY", AccessKey)
        .WithEnvironment("GARAGE_DEFAULT_SECRET_KEY", SecretKey)
        .WithEnvironment("GARAGE_DEFAULT_BUCKET", Bucket)
        .WithCommand("/garage", "server", "--single-node", "--default-bucket")
        .WithPortBinding(S3Port, assignRandomHostPort: true)
        // The image has no shell: wait until the S3 port answers anything over HTTP.
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
            request.ForPort(S3Port).ForStatusCodeMatching(_ => true)))
        .Build();

    public string Endpoint => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(S3Port)}";

    /// <summary>Direct S3 access with the bucket's key, to look behind the port.</summary>
    public IAmazonS3 Client { get; private set; } = null!;

    public IReadOnlyDictionary<string, string?> Settings => new Dictionary<string, string?>
    {
        ["Storage:Endpoint"] = Endpoint,
        ["Storage:Region"] = Region,
        ["Storage:Bucket"] = Bucket,
        ["Storage:AccessKey"] = AccessKey,
        ["Storage:SecretKey"] = SecretKey,
    };

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        Client = new AmazonS3Client(new BasicAWSCredentials(AccessKey, SecretKey), ObjectStorage.ClientConfig(Endpoint, Region));

        // The default bucket appears a moment after the server starts.
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (true)
        {
            try
            {
                await Client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = Bucket, MaxKeys = 1 });
                break;
            }
            catch (AmazonS3Exception) when (DateTime.UtcNow < deadline)
            {
                await Task.Delay(500);
            }
        }

        // What tools/storage/bootstrap.sh applies in every environment.
        await Client.PutLifecycleConfigurationAsync(new PutLifecycleConfigurationRequest
        {
            BucketName = Bucket,
            Configuration = new LifecycleConfiguration
            {
                Rules =
                [
                    new LifecycleRule
                    {
                        Id = "expire-uploads",
                        Status = LifecycleRuleStatus.Enabled,
                        Filter = new LifecycleFilter { LifecycleFilterPredicate = new LifecyclePrefixPredicate { Prefix = ObjectStorageRules.UploadPrefix } },
                        Expiration = new LifecycleRuleExpiration { Days = 1 },
                    },
                ],
            },
        });
    }

    public async ValueTask DisposeAsync()
    {
        Client?.Dispose();
        await _container.DisposeAsync();
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Memorio.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("Memorio.slnx not found above the test output.");
    }
}

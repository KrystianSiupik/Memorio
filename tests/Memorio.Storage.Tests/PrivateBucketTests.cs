using System.Net;
using System.Net.Http.Headers;
using Amazon.S3;
using Memorio.Hosting.Storage;
using Memorio.SharedKernel.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Storage.Tests;

/// <summary>
/// ADR 0005 on a real S3-compatible server: media only through signed addresses (SD5), uploads through
/// the temporary zone, which the bucket expires.
/// </summary>
public sealed class PrivateBucketTests(GarageStorage garage) : IClassFixture<GarageStorage>, IDisposable
{
    private static readonly byte[] Photo = [0xFF, 0xD8, 0xFF, 0xE0, 1, 2, 3, 4];

    private readonly HttpClient _http = new();

    private IObjectStorage Storage => SignedAddressTests.StorageWith(garage.Settings);

    [Fact]
    public async Task An_upload_is_moved_from_the_upload_zone_and_read_through_a_signed_address()
    {
        var slot = await Storage.CreateUploadAsync("image/jpeg", Cancellation);
        using (var upload = new ByteArrayContent(Photo))
        {
            upload.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            using var uploaded = await _http.PutAsync(slot.Upload.Url, upload, Cancellation);
            Assert.Equal(HttpStatusCode.OK, uploaded.StatusCode);
        }

        var destination = new StorageKey($"photos/{Guid.NewGuid():N}/original.jpg");
        var stored = await Storage.PromoteUploadAsync(slot.Key, destination, Cancellation);

        Assert.Equal(new StoredObject(destination, Photo.Length, "image/jpeg"), stored);
        var read = (await Storage.SignReadsAsync([destination], Cancellation))[destination];
        Assert.Equal(Photo, await _http.GetByteArrayAsync(read.Url, Cancellation));
        Assert.Null(await Storage.PromoteUploadAsync(slot.Key, destination, Cancellation));
    }

    [Fact]
    public async Task Nothing_is_readable_without_a_signature()
    {
        var key = await StoreAsync();

        using var anonymousObject = await _http.GetAsync(new Uri($"{garage.Endpoint}/{GarageStorage.Bucket}/{key.Value}"), Cancellation);
        using var anonymousList = await _http.GetAsync(new Uri($"{garage.Endpoint}/{GarageStorage.Bucket}?list-type=2"), Cancellation);

        Assert.Contains(anonymousObject.StatusCode, new[] { HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized });
        Assert.Contains(anonymousList.StatusCode, new[] { HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized });
    }

    [Fact]
    public async Task An_upload_with_another_content_type_than_signed_is_refused()
    {
        var slot = await Storage.CreateUploadAsync("image/jpeg", Cancellation);
        using var upload = new ByteArrayContent(Photo);
        upload.Headers.ContentType = new MediaTypeHeaderValue("text/html");

        using var uploaded = await _http.PutAsync(slot.Upload.Url, upload, Cancellation);

        Assert.Equal(HttpStatusCode.Forbidden, uploaded.StatusCode);
    }

    [Fact]
    public async Task Deleting_is_idempotent()
    {
        var key = await StoreAsync();

        await Storage.DeleteAsync(key, Cancellation);
        await Storage.DeleteAsync(key, Cancellation);

        var exception = await Assert.ThrowsAsync<AmazonS3Exception>(() =>
            garage.Client.GetObjectMetadataAsync(GarageStorage.Bucket, key.Value, Cancellation));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task The_bucket_expires_the_upload_zone_after_a_day()
    {
        var lifecycle = await garage.Client.GetLifecycleConfigurationAsync(GarageStorage.Bucket, Cancellation);

        var rule = Assert.Single(lifecycle.Configuration.Rules);
        Assert.Equal(1, rule.Expiration.Days);
    }

    [Fact]
    public async Task Readiness_follows_the_bucket()
    {
        Assert.Equal(HealthStatus.Healthy, await ReadinessAsync(garage.Settings));

        var missingBucket = new Dictionary<string, string?>(garage.Settings) { ["Storage:Bucket"] = "no-such-bucket" };
        Assert.Equal(HealthStatus.Unhealthy, await ReadinessAsync(missingBucket));
    }

    public void Dispose() => _http.Dispose();

    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task<StorageKey> StoreAsync()
    {
        var slot = await Storage.CreateUploadAsync("image/jpeg", Cancellation);
        using var upload = new ByteArrayContent(Photo);
        upload.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        using var uploaded = await _http.PutAsync(slot.Upload.Url, upload, Cancellation);
        uploaded.EnsureSuccessStatusCode();
        var key = new StorageKey($"photos/{Guid.NewGuid():N}/original.jpg");
        await Storage.PromoteUploadAsync(slot.Key, key, Cancellation);
        return key;
    }

    private static async Task<HealthStatus> ReadinessAsync(IReadOnlyDictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        await using var services = new ServiceCollection()
            .AddLogging()
            .AddMemorioObjectStorage(configuration)
            .BuildServiceProvider();
        var report = await services.GetRequiredService<HealthCheckService>().CheckHealthAsync(Cancellation);
        return report.Entries[ObjectStorage.HealthCheckName].Status;
    }
}

using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Hosting.Storage;

/// <summary>Ready only while the bucket answers (RD4): without it no media can be uploaded or moved.</summary>
internal sealed class ObjectStorageHealthCheck(IAmazonS3 client, string bucket) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = bucket, MaxKeys = 1 }, cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception exception) when (exception is AmazonS3Exception or HttpRequestException or TaskCanceledException)
        {
            return HealthCheckResult.Unhealthy("Object storage unavailable.", exception);
        }
    }
}

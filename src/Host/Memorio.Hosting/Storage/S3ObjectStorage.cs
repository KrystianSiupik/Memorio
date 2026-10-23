using Amazon.S3;
using Amazon.S3.Model;
using Memorio.SharedKernel.Storage;

namespace Memorio.Hosting.Storage;

/// <summary>
/// <see cref="IObjectStorage"/> on any S3-compatible service, path-style. Signing is local (no request);
/// the signer is configured with the public endpoint, the client with the one the hosts reach.
/// </summary>
internal sealed class S3ObjectStorage(IAmazonS3 client, IAmazonS3 signer, string bucket, Protocol publicProtocol, TimeProvider time)
    : IObjectStorage
{
    public async Task<IReadOnlyDictionary<StorageKey, SignedUrl>> SignReadsAsync(
        IReadOnlyCollection<StorageKey> keys,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(keys);
        var expiresAt = time.GetUtcNow() + ObjectStorageRules.SignatureLifetime;
        var signed = new Dictionary<StorageKey, SignedUrl>(keys.Count);
        foreach (var key in keys.Distinct())
        {
            signed[key] = await SignAsync(key, HttpVerb.GET, expiresAt, contentType: null);
        }

        return signed;
    }

    public async Task<UploadSlot> CreateUploadAsync(string contentType, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        var key = new StorageKey($"{ObjectStorageRules.UploadPrefix}{Guid.CreateVersion7():N}");
        var upload = await SignAsync(key, HttpVerb.PUT, time.GetUtcNow() + ObjectStorageRules.SignatureLifetime, contentType);
        return new UploadSlot(key, upload, contentType);
    }

    public async Task<StoredObject?> PromoteUploadAsync(StorageKey upload, StorageKey destination, CancellationToken cancellationToken)
    {
        if (!upload.IsUpload || destination.IsUpload)
        {
            throw new ArgumentException(
                $"Promote moves from {ObjectStorageRules.UploadPrefix} to a final key, not {upload} → {destination}.",
                nameof(destination));
        }

        GetObjectMetadataResponse metadata;
        try
        {
            metadata = await client.GetObjectMetadataAsync(bucket, upload.Value, cancellationToken);
        }
        catch (AmazonS3Exception exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await client.CopyObjectAsync(
            new CopyObjectRequest
            {
                SourceBucket = bucket,
                SourceKey = upload.Value,
                DestinationBucket = bucket,
                DestinationKey = destination.Value,
            },
            cancellationToken);
        await client.DeleteObjectAsync(bucket, upload.Value, cancellationToken);

        return new StoredObject(destination, metadata.ContentLength, metadata.Headers.ContentType ?? "application/octet-stream");
    }

    public Task DeleteAsync(StorageKey key, CancellationToken cancellationToken) =>
        client.DeleteObjectAsync(bucket, key.Value, cancellationToken);

    private async Task<SignedUrl> SignAsync(StorageKey key, HttpVerb verb, DateTimeOffset expiresAt, string? contentType)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = key.Value,
            Verb = verb,
            Expires = expiresAt.UtcDateTime,
            Protocol = publicProtocol,
        };
        if (contentType is not null)
        {
            request.ContentType = contentType;
        }

        return new SignedUrl(new Uri(await signer.GetPreSignedURLAsync(request)), expiresAt);
    }
}

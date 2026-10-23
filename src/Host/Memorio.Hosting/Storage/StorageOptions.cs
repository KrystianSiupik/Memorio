namespace Memorio.Hosting.Storage;

/// <summary>
/// Configuration section "Storage": the private S3-compatible bucket in the EU (ADR 0005). Stage 1: the
/// VPS provider's object storage; stage 2: S3 (D-058). Locally: Garage in Compose.
/// </summary>
public sealed class StorageOptions
{
    public const string Section = "Storage";

    /// <summary>S3 API address the hosts call, e.g. https://fsn1.your-objectstorage.com. Required.</summary>
    public string? Endpoint { get; set; }

    /// <summary>Address the apps reach the storage at, used in signed addresses; defaults to <see cref="Endpoint"/>.</summary>
    public string? PublicEndpoint { get; set; }

    /// <summary>Signing region of the provider, e.g. fsn1 (Hetzner) or eu-central-1 (AWS). Required.</summary>
    public string? Region { get; set; }

    public string? Bucket { get; set; }

    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }
}

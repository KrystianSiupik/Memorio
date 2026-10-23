using Memorio.Hosting.Storage;
using Memorio.SharedKernel.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Storage.Tests;

/// <summary>
/// Fitness function SD5 (ADR 0005), configuration side: every signed address expires within 300 s.
/// Signing is local, so these tests need no storage service.
/// </summary>
public sealed class SignedAddressTests
{
    private static readonly IObjectStorage Storage = StorageWith(new Dictionary<string, string?>
    {
        ["Storage:Endpoint"] = "http://garage:3900",
        ["Storage:PublicEndpoint"] = "https://media.example.eu",
        ["Storage:Region"] = "fsn1",
        ["Storage:Bucket"] = "memorio-media",
        ["Storage:AccessKey"] = "test-access-key",
        ["Storage:SecretKey"] = "test-secret-key",
    });

    [Fact]
    public void The_signature_lifetime_is_at_most_300_seconds()
    {
        Assert.True(ObjectStorageRules.SignatureLifetime <= TimeSpan.FromSeconds(300));
    }

    [Fact]
    public async Task A_batch_of_reads_is_signed_at_once_for_the_public_address_and_expires_within_300_seconds()
    {
        StorageKey first = new("photos/a/original.jpg"), second = new("photos/b/original.jpg");
        var before = DateTimeOffset.UtcNow;

        var signed = await Storage.SignReadsAsync([first, second, first], TestContext.Current.CancellationToken);

        Assert.Equal([first, second], signed.Keys.Order(Comparer<StorageKey>.Create((x, y) => string.CompareOrdinal(x.Value, y.Value))));
        foreach (var (key, url) in signed)
        {
            Assert.Equal("https", url.Url.Scheme);
            Assert.Equal("media.example.eu", url.Url.Host);
            Assert.Equal($"/memorio-media/{key.Value}", url.Url.AbsolutePath);
            Assert.True(ExpiresInSeconds(url.Url) <= 300);
            Assert.InRange(url.ExpiresAt, before, before + ObjectStorageRules.SignatureLifetime + TimeSpan.FromSeconds(5));
        }

        Assert.Single(signed.Values.Select(url => url.ExpiresAt).Distinct());
    }

    [Fact]
    public async Task An_upload_goes_to_a_new_key_in_the_upload_zone_and_its_content_type_is_signed()
    {
        var slot = await Storage.CreateUploadAsync("image/jpeg", TestContext.Current.CancellationToken);
        var other = await Storage.CreateUploadAsync("image/jpeg", TestContext.Current.CancellationToken);

        Assert.True(slot.Key.IsUpload);
        Assert.NotEqual(slot.Key, other.Key);
        Assert.True(ExpiresInSeconds(slot.Upload.Url) <= 300);
        Assert.Contains("content-type", Query(slot.Upload.Url)["X-Amz-SignedHeaders"], StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("photos/a.jpg", "photos/b.jpg")]
    [InlineData("uploads/a", "uploads/b")]
    public async Task Promote_only_moves_from_the_upload_zone_to_a_final_key(string upload, string destination)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Storage.PromoteUploadAsync(new StorageKey(upload), new StorageKey(destination), TestContext.Current.CancellationToken));
    }

    internal static IObjectStorage StorageWith(IReadOnlyDictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        return new ServiceCollection()
            .AddMemorioObjectStorage(configuration)
            .BuildServiceProvider()
            .GetRequiredService<IObjectStorage>();
    }

    private static int ExpiresInSeconds(Uri url) => int.Parse(Query(url)["X-Amz-Expires"], System.Globalization.CultureInfo.InvariantCulture);

    private static Dictionary<string, string> Query(Uri url) =>
        url.Query.TrimStart('?').Split('&')
            .Select(pair => pair.Split('=', 2))
            .ToDictionary(pair => Uri.UnescapeDataString(pair[0]), pair => Uri.UnescapeDataString(pair[1]));
}

using Memorio.SharedKernel.Storage;

namespace Memorio.Storage.Tests;

public sealed class StorageKeyTests
{
    [Theory]
    [InlineData("photos/0192a3b4/original.jpg")]
    [InlineData("uploads/0192a3b4c5d6")]
    [InlineData("death-certificates/a_b.pdf")]
    public void A_relative_key_of_safe_segments_is_valid(string value)
    {
        Assert.Equal(value, new StorageKey(value).Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("/photos/a.jpg")]
    [InlineData("photos//a.jpg")]
    [InlineData("photos/../secrets")]
    [InlineData("photos/./a.jpg")]
    [InlineData("photos/a b.jpg")]
    [InlineData("photos/a?x=1")]
    [InlineData("photos\\a.jpg")]
    public void Anything_else_is_rejected(string value)
    {
        Assert.Throws<ArgumentException>(() => new StorageKey(value));
    }

    [Fact]
    public void Only_keys_under_the_upload_prefix_are_uploads()
    {
        Assert.True(new StorageKey("uploads/abc").IsUpload);
        Assert.False(new StorageKey("photos/uploads/abc").IsUpload);
    }
}

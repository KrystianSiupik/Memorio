using Memorio.Api.AppVersion;

namespace Memorio.Api.Tests;

public sealed class AppVersionNumberTests
{
    [Theory]
    [InlineData("1.4.0", 1, 4, 0)]
    [InlineData(" 1.4.0 ", 1, 4, 0)]
    [InlineData("1.4.0+12", 1, 4, 0)]
    [InlineData("2.0.10-beta", 2, 0, 10)]
    public void A_version_is_major_minor_patch_without_suffixes(string text, int major, int minor, int patch)
    {
        Assert.True(AppVersionNumber.TryParse(text, out var version));
        Assert.Equal(new AppVersionNumber(major, minor, patch), version);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1.4")]
    [InlineData("1.4.0.1")]
    [InlineData("1.x.0")]
    [InlineData("-1.4.0")]
    [InlineData("1. 4.0")]
    public void Anything_else_is_not_a_version(string? text)
    {
        Assert.False(AppVersionNumber.TryParse(text, out _));
    }

    [Fact]
    public void Versions_compare_by_number_not_by_text()
    {
        Assert.True(new AppVersionNumber(1, 10, 0) > new AppVersionNumber(1, 9, 0));
        Assert.True(new AppVersionNumber(1, 9, 9) < new AppVersionNumber(2, 0, 0));
        Assert.True(new AppVersionNumber(1, 2, 3) >= new AppVersionNumber(1, 2, 3));
    }
}

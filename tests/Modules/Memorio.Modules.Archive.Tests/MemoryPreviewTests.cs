using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Memorio.Modules.Archive.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E12-PLAT-01: the preview on the map, with the access of the details.</summary>
public sealed class MemoryPreviewTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_author_sees_the_place_the_date_and_the_first_photo_signed()
    {
        var author = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author, photos: 3, dateFrom: new DateOnly(2026, 7, 1), time: "09:30");
        await using var scope = database.CreateScope();

        var preview = await scope.ServiceProvider.GetRequiredService<IMemoryDetailsQuery>().PreviewAsync(memory.Id.Value, author, Cancellation);

        Assert.NotNull(preview);
        Assert.Equal("Giewont", preview.PlaceName);
        Assert.Equal("2026-07-01", preview.DateFrom);
        Assert.Equal("09:30", preview.Time);
        Assert.Equal(3, preview.Photos);
        Assert.NotNull(preview.PhotoUrl);
        Assert.Contains(memory.Photos.OrderBy(photo => photo.Id).First().StorageKey, preview.PhotoUrl.ToString(), StringComparison.Ordinal);
        Assert.Null(preview.Excerpt);
    }

    [Fact]
    public async Task Somebody_else_sees_it_only_through_an_album_and_never_when_hidden()
    {
        var (author, reader, album) = (Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        var memory = await database.AddMemoryAsync(author);
        await using var scope = database.CreateScope();
        var query = scope.ServiceProvider.GetRequiredService<IMemoryDetailsQuery>();

        Assert.Null(await query.PreviewAsync(memory.Id.Value, reader, Cancellation));

        await scope.ServiceProvider.GetRequiredService<IAlbumContent>().LinkAsync(memory.Id.Value, album, Cancellation);
        database.Neighbours.Sees(reader, album);
        Assert.NotNull(await query.PreviewAsync(memory.Id.Value, reader, Cancellation));

        database.Neighbours.Hides(memory.Id.Value);
        Assert.Null(await query.PreviewAsync(memory.Id.Value, reader, Cancellation));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("   ", null)]
    [InlineData("Krótki opis.", "Krótki opis.")]
    public void A_short_description_is_shown_whole(string? description, string? excerpt) =>
        Assert.Equal(excerpt, MemoryPreview.ExcerptOf(description));

    [Fact]
    public void A_long_description_is_cut_at_a_word()
    {
        var long_ = string.Join(' ', Enumerable.Repeat("słowo", 60));

        var excerpt = MemoryPreview.ExcerptOf(long_)!;

        Assert.True(excerpt.Length <= MemoryPreview.ExcerptLength + 1);
        Assert.EndsWith("słowo…", excerpt, StringComparison.Ordinal);
    }
}

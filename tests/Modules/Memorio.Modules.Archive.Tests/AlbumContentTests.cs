using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Memorio.Modules.Archive.Application.Queries.MemoryMap;
using Memorio.Modules.Archive.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E3-ARCHIWUM-01 and D-231 closed for albums, on a real database.</summary>
public sealed class AlbumContentTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_memory_in_an_album_is_seen_only_while_the_person_may_see_the_album()
    {
        var author = Guid.CreateVersion7();
        var reader = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author);
        await using var scope = database.CreateScope();
        var details = scope.ServiceProvider.GetRequiredService<IMemoryDetailsQuery>();
        var map = scope.ServiceProvider.GetRequiredService<IMemoryMapQuery>();
        var content = scope.ServiceProvider.GetRequiredService<IAlbumContent>();

        database.Neighbours.Sees(reader, album);
        Assert.Null(await details.GetAsync(memory.Id.Value, reader, Cancellation));

        await content.LinkAsync(memory.Id.Value, album, Cancellation);
        await content.LinkAsync(memory.Id.Value, album, Cancellation);

        Assert.NotNull(await details.GetAsync(memory.Id.Value, reader, Cancellation));
        Assert.Contains(await map.GetAsync(reader, null, null, Cancellation), point => point.Id == memory.Id.Value);

        database.Neighbours.NoLongerSees(reader, album);

        Assert.Null(await details.GetAsync(memory.Id.Value, reader, Cancellation));
        Assert.DoesNotContain(await map.GetAsync(reader, null, null, Cancellation), point => point.Id == memory.Id.Value);
    }

    [Fact]
    public async Task Facts_are_only_about_active_memories()
    {
        var author = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author, photos: 2);
        await using var scope = database.CreateScope();
        var content = scope.ServiceProvider.GetRequiredService<IAlbumContent>();

        var facts = await content.DescribeAsync(memory.Id.Value, Cancellation);
        Assert.Equal(author, facts!.AuthorAccountId);
        Assert.Equal(200, facts.SizeBytes);
        Assert.Equal(2, (await content.SummarizeAsync([memory.Id.Value], Cancellation))[memory.Id.Value].Photos);

        await database.TrashAsync(memory.Id, author);

        Assert.Null(await DescribeFreshAsync(memory.Id.Value));
        Assert.Empty(await SummarizeFreshAsync(memory.Id.Value));
    }

    [Fact]
    public async Task A_photo_in_an_album_is_paid_by_its_pool_and_goes_back_to_the_author_when_the_memory_leaves()
    {
        var author = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author);
        var photo = Guid.CreateVersion7();
        var upload = database.Storage.AddUpload(50);
        await using (var scope = database.CreateScope())
        {
            var content = scope.ServiceProvider.GetRequiredService<IAlbumContent>();
            await content.LinkAsync(memory.Id.Value, album, Cancellation);

            var added = await content.AddPhotoAsync(new AlbumPhoto(album, memory.Id.Value, author, photo, upload.Value), Cancellation);

            Assert.Equal(AlbumPhotoOutcome.Added, added);
            Assert.Equal(AlbumPhotoOutcome.AlreadyThere, await content.AddPhotoAsync(new AlbumPhoto(album, memory.Id.Value, author, photo, upload.Value), Cancellation));
            Assert.Equal(50, database.Neighbours.Reserved[photo]);
        }

        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IAlbumContent>().UnlinkAsync(memory.Id.Value, album, Cancellation);
        }

        Assert.Equal(author, database.Neighbours.PayerOf(photo));
        Assert.Equal(50, database.Neighbours.Reserved[photo]);
    }

    [Fact]
    public async Task The_authors_space_comes_back_while_an_album_pays_and_returns_with_the_last_one()
    {
        var author = Guid.CreateVersion7();
        var (first, second) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        var memory = await database.AddMemoryAsync(author);
        var photo = await AddPersonalPhotoAsync(memory.Id, author);
        Assert.Equal(author, database.Neighbours.PayerOf(photo));

        await WithContentAsync(content => content.LinkAsync(memory.Id.Value, first, Cancellation));
        Assert.Null(database.Neighbours.PayerOf(photo));

        await WithContentAsync(content => content.LinkAsync(memory.Id.Value, second, Cancellation));
        await WithContentAsync(content => content.UnlinkAsync(memory.Id.Value, first, Cancellation));
        Assert.Null(database.Neighbours.PayerOf(photo));

        await WithContentAsync(content => content.UnlinkAsync(memory.Id.Value, second, Cancellation));
        Assert.Equal(author, database.Neighbours.PayerOf(photo));

        await WithContentAsync(content => content.UnlinkAsync(memory.Id.Value, second, Cancellation));
        Assert.Equal(author, database.Neighbours.PayerOf(photo));
    }

    [Fact]
    public async Task Taking_a_memory_along_counts_even_over_the_authors_limit()
    {
        var author = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author);
        var photo = await AddPersonalPhotoAsync(memory.Id, author);
        await WithContentAsync(content => content.LinkAsync(memory.Id.Value, album, Cancellation));
        database.Neighbours.WithFullPool(author);

        await WithContentAsync(content => content.UnlinkAsync(memory.Id.Value, album, Cancellation));

        Assert.Equal(author, database.Neighbours.PayerOf(photo));
    }

    [Fact]
    public async Task A_photo_paid_by_one_album_goes_to_the_author_when_only_that_album_lets_go()
    {
        var author = Guid.CreateVersion7();
        var (paying, other) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        var memory = await database.AddMemoryAsync(author);
        var photo = Guid.CreateVersion7();
        await WithContentAsync(async content =>
        {
            await content.LinkAsync(memory.Id.Value, paying, Cancellation);
            await content.LinkAsync(memory.Id.Value, other, Cancellation);
            await content.AddPhotoAsync(new AlbumPhoto(paying, memory.Id.Value, author, photo, database.Storage.AddUpload(50).Value), Cancellation);
        });
        Assert.Equal(paying, database.Neighbours.PayerOf(photo));

        await WithContentAsync(content => content.UnlinkAsync(memory.Id.Value, paying, Cancellation));

        Assert.Equal(author, database.Neighbours.PayerOf(photo));
    }

    private async Task<Guid> AddPersonalPhotoAsync(Domain.Memories.MemoryId memory, Guid author)
    {
        var photo = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var added = await scope.ServiceProvider.GetRequiredService<Application.Commands.AddPhoto.AddPhotoHandler>()
            .HandleAsync(new Application.Commands.AddPhoto.AddPhoto(memory, author, photo, database.Storage.AddUpload(50).Value), Cancellation);
        Assert.True(added.IsSuccess);
        return photo;
    }

    private async Task WithContentAsync(Func<IAlbumContent, Task> action)
    {
        await using var scope = database.CreateScope();
        await action(scope.ServiceProvider.GetRequiredService<IAlbumContent>());
    }

    [Fact]
    public async Task A_full_album_pool_refuses_and_keeps_nothing()
    {
        var author = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author);
        database.Neighbours.WithFullPool(album);
        await using var scope = database.CreateScope();
        var content = scope.ServiceProvider.GetRequiredService<IAlbumContent>();

        var refused = await content.AddPhotoAsync(new AlbumPhoto(album, memory.Id.Value, author, Guid.CreateVersion7(), database.Storage.AddUpload(50).Value), Cancellation);

        Assert.Equal(AlbumPhotoOutcome.PoolFull, refused);
        Assert.DoesNotContain((await content.SummarizeAsync([memory.Id.Value], Cancellation)).Values, summary => summary.Photos > 0);
    }

    [Fact]
    public async Task Someone_else_cannot_add_to_a_memory()
    {
        var memory = await database.AddMemoryAsync(Guid.CreateVersion7());
        await using var scope = database.CreateScope();

        var refused = await scope.ServiceProvider.GetRequiredService<IAlbumContent>().AddPhotoAsync(
            new AlbumPhoto(Guid.CreateVersion7(), memory.Id.Value, Guid.CreateVersion7(), Guid.CreateVersion7(), database.Storage.AddUpload(1).Value),
            Cancellation);

        Assert.Equal(AlbumPhotoOutcome.NoAccess, refused);
    }

    private async Task<AlbumMemoryFacts?> DescribeFreshAsync(Guid memoryId)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAlbumContent>().DescribeAsync(memoryId, Cancellation);
    }

    private async Task<IReadOnlyDictionary<Guid, AlbumMemorySummary>> SummarizeFreshAsync(Guid memoryId)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAlbumContent>().SummarizeAsync([memoryId], Cancellation);
    }
}

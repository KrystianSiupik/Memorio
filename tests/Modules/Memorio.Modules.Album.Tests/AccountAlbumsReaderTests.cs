using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>MEM-E1-ALBUM-01 on a real database: the albums of an account, and only of that account.</summary>
public sealed class AccountAlbumsReaderTests(AlbumDatabase database)
{
    [Fact]
    public async Task Tells_own_albums_those_without_a_successor_and_other_peoples_albums_apart()
    {
        var account = Guid.CreateVersion7();
        var someoneElse = Guid.CreateVersion7();
        await database.AddAsync(SharedAlbum.Create(account));
        await database.AddAsync(SharedAlbum.Create(account, successorAccountId: someoneElse));
        await database.AddAsync(WithParticipant(SharedAlbum.Create(someoneElse), account));
        await database.AddAsync(SharedAlbum.Create(someoneElse));

        Assert.Equal(new AccountAlbumsSummary(OwnAlbums: 2, OwnAlbumsWithoutSuccessor: 1, OtherPeoplesAlbums: 1), await SummarizeAsync(account));
    }

    [Fact]
    public async Task Being_the_successor_of_an_album_does_not_count_as_taking_part()
    {
        var account = Guid.CreateVersion7();
        await database.AddAsync(SharedAlbum.Create(Guid.CreateVersion7(), successorAccountId: account));

        Assert.Equal(new AccountAlbumsSummary(0, 0, 0), await SummarizeAsync(account));
    }

    [Fact]
    public async Task An_owner_listed_in_their_own_album_counts_once()
    {
        var account = Guid.CreateVersion7();
        await database.AddAsync(WithParticipant(SharedAlbum.Create(account), account));

        Assert.Equal(new AccountAlbumsSummary(1, 1, 0), await SummarizeAsync(account));
    }

    private static SharedAlbum WithParticipant(SharedAlbum album, Guid participant)
    {
        album.AddParticipant(participant);
        return album;
    }

    private async Task<AccountAlbumsSummary> SummarizeAsync(Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAccountAlbumsReader>()
            .SummarizeAsync(account, TestContext.Current.CancellationToken);
    }
}

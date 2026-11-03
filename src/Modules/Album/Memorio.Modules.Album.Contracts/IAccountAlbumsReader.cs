namespace Memorio.Modules.Album.Contracts;

/// <summary>Read port for the Account module: the albums a person is in, and which would be left without an owner.</summary>
public interface IAccountAlbumsReader
{
    Task<AccountAlbumsSummary> SummarizeAsync(Guid accountId, CancellationToken cancellationToken);
}

/// <param name="OwnAlbums">Albums the person owns.</param>
/// <param name="OwnAlbumsWithoutSuccessor">Of those, albums with no successor named.</param>
/// <param name="OtherPeoplesAlbums">Albums of other owners the person takes part in.</param>
public sealed record AccountAlbumsSummary(int OwnAlbums, int OwnAlbumsWithoutSuccessor, int OtherPeoplesAlbums);

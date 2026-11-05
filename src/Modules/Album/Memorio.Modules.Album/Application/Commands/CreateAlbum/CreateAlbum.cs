using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Album.Application.Commands.CreateAlbum;

/// <summary>C40. The id comes from the client: sending it again finds the same album.</summary>
internal sealed record CreateAlbum(Guid Id, Guid AccountId, string? Name);

internal sealed class CreateAlbumHandler(IAlbums albums, IAccountDirectory directory, TimeProvider clock)
{
    public async Task<Result> HandleAsync(CreateAlbum command, CancellationToken cancellationToken)
    {
        var id = new AlbumId(command.Id);
        var started = SharedAlbum.Start(id, command.AccountId, command.Name, clock.GetUtcNow());
        if (started.IsFailure)
        {
            return started.Error;
        }

        if (await albums.FindAsync(id, cancellationToken) is { } existing)
        {
            return existing.IsOwner(command.AccountId) ? Result.Success() : AlbumErrors.IdTaken;
        }

        // An album's pool and plan live under its id (D-246), so it must never be somebody's account id (D-283).
        if (await directory.EmailOfAsync(command.Id, cancellationToken) is not null)
        {
            return AlbumErrors.IdTaken;
        }

        albums.Add(started.Value);
        if (await albums.SaveAsync(cancellationToken))
        {
            return Result.Success();
        }

        // The same request twice at once.
        var winner = await albums.FindAsync(id, cancellationToken);
        return winner?.IsOwner(command.AccountId) == true ? Result.Success() : AlbumErrors.IdTaken;
    }
}

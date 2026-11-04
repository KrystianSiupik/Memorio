using Memorio.Modules.Album.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Succession.Application.Commands.NameSuccessor;

/// <summary>C41: the owner names who takes the album over. Who is in the album, and so who may be named, is Album's to say.</summary>
internal sealed record NameSuccessor(Guid AlbumId, Guid OwnerAccountId, Guid SuccessorAccountId);

internal sealed class NameSuccessorHandler(IAlbumSuccession albums)
{
    public static readonly Error NoAccess = Error.Forbidden("Succession.NoAccess", "No access to this album.");

    public static readonly Error NotParticipant = Error.Validation("Succession.NotParticipant", "A successor has to be in the album.");

    public static readonly Error AlreadyDecided = Error.Conflict("Succession.AlreadyDecided", "This has already been decided.");

    public async Task<Result> HandleAsync(NameSuccessor command, CancellationToken cancellationToken) =>
        await albums.NameAsync(command.AlbumId, command.OwnerAccountId, command.SuccessorAccountId, cancellationToken) switch
        {
            SuccessorOutcome.Named => Result.Success(),
            SuccessorOutcome.NotParticipant => NotParticipant,
            SuccessorOutcome.AlreadyDecided => AlreadyDecided,
            _ => NoAccess,
        };
}

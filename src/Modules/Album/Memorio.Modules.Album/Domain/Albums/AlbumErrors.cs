using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Album.Domain.Albums;

internal static class AlbumErrors
{
    public static readonly Error InvalidName = Error.Validation("Album.InvalidName", $"An album has a name, up to {SharedAlbum.MaximumNameLength} characters.");

    public static readonly Error InvalidEmail = Error.Validation("Album.InvalidEmail", "This is not an e-mail address.");

    public static readonly Error CannotInviteYourself = Error.Validation("Album.CannotInviteYourself", "The owner is already in the album.");

    /// <summary>E43.</summary>
    public static readonly Error NoFreeSeats = Error.Conflict("Album.NoFreeSeats", "The plan of this album has no free seat.");

    /// <summary>E44.</summary>
    public static readonly Error AddressBlocked = Error.Conflict("Album.AddressBlocked", "This address does not take invitations.");

    /// <summary>The answer for an invitation answered, taken back or never made for this person.</summary>
    public static readonly Error InvitationGone = Error.Conflict("Album.InvitationGone", "The invitation has expired or was withdrawn.");

    public static readonly Error OwnerCannotLeave = Error.Conflict("Album.OwnerCannotLeave", "The owner cannot leave or be removed.");

    public static readonly Error SuccessorNotParticipant = Error.Validation("Album.SuccessorNotParticipant", "A successor has to be in the album.");

    public static readonly Error NotParticipant = Error.Validation("Album.NotParticipant", "This person is not in the album.");

    public static readonly Error CannotEntrustToYourself = Error.Validation("Album.CannotEntrustToYourself", "A curator is somebody else.");

    public static readonly Error InvalidArrangement = Error.Validation("Album.InvalidArrangement", "An arrangement names memories entrusted to you, each once.");

    /// <summary>E51.</summary>
    public static readonly Error PoolFull = Error.Conflict("Album.PoolFull", "The pool of this album is full.");

    public static readonly Error UploadMissing = Error.NotFound("Album.UploadMissing", "The upload does not exist; it may have expired.");

    public static readonly Error IdTaken = Error.Conflict("Album.IdTaken", "This id belongs to another album.");

    /// <summary>Somebody got there first.</summary>
    public static readonly Error AlreadyDecided = Error.Conflict("Album.AlreadyDecided", "This has already been decided.");

    /// <summary>The same answer for an album that does not exist and one the person is not in.</summary>
    public static readonly Error NoAccess = Error.Forbidden("Album.NoAccess", "No access to this album.");
}

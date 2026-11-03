using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Domain.Memories;

internal static class MemoryErrors
{
    public static readonly Error InvalidDate = Error.Validation("Archive.InvalidDate", "The date is not valid.");

    /// <summary>E13.</summary>
    public static readonly Error DateInFuture = Error.Validation("Archive.DateInFuture", "A memory cannot happen in the future.");

    public static readonly Error PlaceNotFound = Error.Validation("Archive.PlaceNotFound", "The place of the memory does not exist.");

    public static readonly Error TermsNotAccepted = Error.Conflict("Archive.TermsNotAccepted", "The terms come before the first content.");

    /// <summary>E15.</summary>
    public static readonly Error SpaceLimitExceeded = Error.Conflict("Archive.SpaceLimitExceeded", "There is no room left for this photo.");

    public static readonly Error InvalidUpload = Error.Validation("Archive.InvalidUpload", "This is not an upload.");

    public static readonly Error UploadMissing = Error.NotFound("Archive.UploadMissing", "The upload does not exist; it may have expired.");

    public static readonly Error InvalidContentType = Error.Validation("Archive.InvalidContentType", "Only JPEG, PNG, WebP and HEIC photos are accepted.");

    /// <summary>Somebody got there first: the state of this memory changed under this operation.</summary>
    public static readonly Error AlreadyDecided = Error.Conflict("Archive.AlreadyDecided", "This has already been decided.");

    public static readonly Error InvalidDescription = Error.Validation("Archive.InvalidDescription", $"A description has some text, up to {Memory.MaximumDescriptionLength} characters.");

    public static readonly Error DraftNotFound = Error.Validation("Archive.DraftNotFound", "This is not a draft of this person.");

    public static readonly Error IdTaken = Error.Conflict("Archive.IdTaken", "This id belongs to another memory.");

    /// <summary>The same answer for a memory that does not exist and one of somebody else: no way to tell them apart.</summary>
    public static readonly Error NoAccess = Error.Forbidden("Archive.NoAccess", "No access to this memory.");
}

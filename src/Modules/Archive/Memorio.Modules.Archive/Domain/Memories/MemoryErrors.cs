using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Domain.Memories;

internal static class MemoryErrors
{
    public static readonly Error InvalidDate = Error.Validation("Archive.InvalidDate", "The date is not valid.");

    /// <summary>E13.</summary>
    public static readonly Error DateInFuture = Error.Validation("Archive.DateInFuture", "A memory cannot happen in the future.");

    public static readonly Error PlaceNotFound = Error.Validation("Archive.PlaceNotFound", "The place of the memory does not exist.");

    public static readonly Error TermsNotAccepted = Error.Conflict("Archive.TermsNotAccepted", "The terms come before the first content.");

    public static readonly Error IdTaken = Error.Conflict("Archive.IdTaken", "This id belongs to another memory.");

    /// <summary>The same answer for a memory that does not exist and one of somebody else: no way to tell them apart.</summary>
    public static readonly Error NoAccess = Error.Forbidden("Archive.NoAccess", "No access to this memory.");
}

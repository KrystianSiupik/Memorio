namespace Memorio.Modules.Access.Contracts;

/// <summary>
/// Write port for Moderation (C67, MEM-E8-DOSTEP-02): hides at once and answers whether it is stored, because an
/// order to remove content must be done within an hour (L11). The event of the same hold changes nothing after it.
/// </summary>
public interface IContentHiding
{
    /// <summary>True once nobody but the author sees the resource. An older <see cref="HiddenContent.Sequence"/> changes nothing.</summary>
    Task<bool> HideAsync(HiddenContent content, CancellationToken cancellationToken);

    /// <summary>The hold ends; other holds of the same resource still stand.</summary>
    Task<bool> ReleaseAsync(Guid holdId, Guid resourceId, int sequence, CancellationToken cancellationToken);
}

/// <param name="HoldId">The report or order that hides it; one resource may have several.</param>
/// <param name="ResourceKind">Memory or Album.</param>
/// <param name="AuthorAccountId">Who still sees it: the author of the memory, the owner of the album.</param>
public sealed record HiddenContent(Guid HoldId, string ResourceKind, Guid ResourceId, Guid? AuthorAccountId, int Sequence);

using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Access.Domain.Audiences;

/// <summary>
/// E161: one hold of Moderation on one resource (a report or an order, S5 A9). While any hold of a resource is hidden,
/// nobody but its author sees it. A later <see cref="Sequence"/> of the same hold wins (D-013), so an event delivered
/// late or twice changes nothing.
/// </summary>
internal sealed record HiddenScope(Guid HoldId, string ResourceKind, Guid ResourceId, Guid? AuthorAccountId, bool Hidden, int Sequence, DateTimeOffset ChangedAt)
{
    public const string Memory = "Memory";

    public const string Album = "Album";

    /// <summary>C67.</summary>
    public static Result<HiddenScope> Hide(Guid holdId, string? resourceKind, Guid resourceId, Guid? authorAccountId, int sequence, DateTimeOffset now) =>
        holdId == Guid.Empty || resourceId == Guid.Empty || sequence < 1 || resourceKind is not (Memory or Album)
            ? AccessErrors.InvalidScope
            : new HiddenScope(holdId, resourceKind, resourceId, authorAccountId, Hidden: true, sequence, now);

    /// <summary>The hold ends. The kind and the author stay unknown when the release came before the hiding.</summary>
    public static Result<HiddenScope> Release(Guid holdId, Guid resourceId, int sequence, DateTimeOffset now) =>
        holdId == Guid.Empty || resourceId == Guid.Empty || sequence < 1
            ? AccessErrors.InvalidScope
            : new HiddenScope(holdId, "", resourceId, null, Hidden: false, sequence, now);
}

internal interface IHiddenScopes
{
    /// <summary>Stores the scope unless the hold already has this or a later sequence. True when it stands after the call.</summary>
    Task<bool> ApplyAsync(HiddenScope scope, CancellationToken cancellationToken);
}

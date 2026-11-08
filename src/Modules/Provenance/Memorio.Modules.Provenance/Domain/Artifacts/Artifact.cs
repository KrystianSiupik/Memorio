using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Provenance.Domain.Artifacts;

/// <summary>
/// AG: something made from people's content (an AI summary, later a public copy or a trip plan) and the list of what it
/// was made from (S2:A2, B2, C5). It exists only while every source may still be used: once invalidated it stays so,
/// also after the person is let back in (B2: derived artifacts do not come back).
/// </summary>
internal sealed class Artifact : AggregateRoot<Guid>
{
    public const int MaxKindLength = 50;

    /// <summary>A summary of a long period cites many memories; past this the list is a mistake, not a story.</summary>
    public const int MaxSources = 1000;

    private readonly List<ArtifactSource> _sources = [];

    private Artifact(Guid id, string kind, Guid ownerAccountId, Guid? albumId, DateTimeOffset createdAt)
        : base(id)
    {
        Kind = kind;
        OwnerAccountId = ownerAccountId;
        AlbumId = albumId;
        CreatedAt = createdAt;
    }

    public string Kind { get; private set; }

    /// <summary>The person the artifact was made for.</summary>
    public Guid OwnerAccountId { get; private set; }

    /// <summary>The album it was made from, when it was made from one.</summary>
    public Guid? AlbumId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public ArtifactState State { get; private set; } = ArtifactState.Valid;

    public DateTimeOffset? InvalidatedAt { get; private set; }

    public InvalidationReason? Reason { get; private set; }

    public int Sequence { get; private set; } = 1;

    public IReadOnlyList<ArtifactSource> Sources => _sources;

    /// <summary>C154.</summary>
    public static Result<Artifact> Register(
        Guid id,
        string? kind,
        Guid ownerAccountId,
        Guid? albumId,
        IReadOnlyCollection<ArtifactSource> sources,
        DateTimeOffset now)
    {
        if (id == Guid.Empty || ownerAccountId == Guid.Empty || albumId == Guid.Empty)
        {
            return ArtifactErrors.InvalidIds;
        }

        if (string.IsNullOrWhiteSpace(kind) || kind.Length > MaxKindLength)
        {
            return ArtifactErrors.InvalidKind;
        }

        if (sources.Count is 0 or > MaxSources
            || sources.Any(source => source.MemoryId == Guid.Empty || source.AuthorAccountId == Guid.Empty || source.AlbumId == Guid.Empty))
        {
            return ArtifactErrors.InvalidSources;
        }

        var artifact = new Artifact(id, kind.Trim(), ownerAccountId, albumId, now);
        artifact._sources.AddRange(sources.DistinctBy(source => source.MemoryId));
        artifact.Raise(new ArtifactRegistered(Guid.CreateVersion7(), now, id, artifact.Sequence));
        return artifact;
    }

    /// <summary>C31: once. False when it already was.</summary>
    public bool Invalidate(InvalidationReason reason, DateTimeOffset now)
    {
        if (State == ArtifactState.Invalidated)
        {
            return false;
        }

        State = ArtifactState.Invalidated;
        InvalidatedAt = now;
        Reason = reason;
        Sequence++;
        Raise(new ArtifactInvalidated(Guid.CreateVersion7(), now, Id, Kind, OwnerAccountId, reason, Sequence));
        return true;
    }
}

/// <summary>One piece of content an artifact was made from.</summary>
/// <param name="AlbumId">The album through which the content was used; null for one's own private content.</param>
internal sealed record ArtifactSource(Guid MemoryId, Guid AuthorAccountId, Guid? AlbumId);

internal enum ArtifactState
{
    Valid,
    Invalidated,
}

/// <summary>Why an artifact may no longer exist: the policies P4, P9, P10, P12 and P34.</summary>
internal enum InvalidationReason
{
    ContentDeleted,
    ContributionsWithdrawn,
    AccessRevoked,
    AiConsentRevoked,
    ArchiveTrimmed,
}

internal sealed record ArtifactRegistered(Guid EventId, DateTimeOffset OccurredAt, Guid ArtifactId, int Sequence) : IDomainEvent;

internal sealed record ArtifactInvalidated(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid ArtifactId,
    string Kind,
    Guid OwnerAccountId,
    InvalidationReason Reason,
    int Sequence) : IDomainEvent;

internal static class ArtifactErrors
{
    public static readonly Error InvalidIds = Error.Validation("Provenance.InvalidIds", "An artifact needs its id and the account it was made for.");

    public static readonly Error InvalidKind = Error.Validation("Provenance.InvalidKind", "An artifact needs a kind of up to 50 characters.");

    public static readonly Error InvalidSources = Error.Validation("Provenance.InvalidSources", "An artifact needs from 1 to 1000 sources, each with its memory and author.");

    public static readonly Error IdTaken = Error.Conflict("Provenance.IdTaken", "This id belongs to another artifact.");
}

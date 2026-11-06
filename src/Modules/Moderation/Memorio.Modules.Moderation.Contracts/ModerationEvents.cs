using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Moderation.Contracts;

/// <summary>What a decision of the service can hide: a memory, or a whole album.</summary>
public static class ModeratedResources
{
    public const string Memory = "Memory";

    public const string Album = "Album";
}

/// <summary>
/// E113: nobody but its author sees this resource any more, while <paramref name="HoldId"/> stands (a report or an
/// order). Only the effect, not why (D-001). <paramref name="Sequence"/> orders the events of one hold (D-013).
/// </summary>
public sealed record ContentHiddenEvent(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid HoldId,
    string ResourceKind,
    Guid ResourceId,
    Guid? AuthorAccountId,
    int Sequence) : IIntegrationEvent;

/// <summary>The hold ended: a decision kept the content, or an appeal reversed the removal.</summary>
public sealed record ContentRestoredEvent(Guid EventId, DateTimeOffset OccurredAt, Guid HoldId, Guid ResourceId, int Sequence) : IIntegrationEvent;

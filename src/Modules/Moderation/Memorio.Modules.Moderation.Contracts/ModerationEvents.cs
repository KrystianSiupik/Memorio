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

/// <summary>
/// E110: a report came. <paramref name="Category"/> is Illegal or Disputed; illegal content is hidden at once
/// (<paramref name="HiddenNow"/>). Who reported is never in it: the author does not learn it (S2:C2).
/// </summary>
public sealed record ContentReportedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ReportId, string Category, Guid? AuthorAccountId, bool HiddenNow, int Sequence) : IIntegrationEvent;

/// <summary>E112: decided with reasons for both sides; <paramref name="Verdict"/> is Removed or Kept. The reasons come from <see cref="IReportNotices"/>.</summary>
public sealed record ReportDecidedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ReportId, string Verdict, Guid? AuthorAccountId, int Sequence) : IIntegrationEvent;

/// <summary>The author's appeal is decided; reversed shows the content again.</summary>
public sealed record AppealDecidedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ReportId, bool Reversed, Guid AuthorAccountId, int Sequence) : IIntegrationEvent;

/// <summary>The hold ended: a decision kept the content, or an appeal reversed the removal.</summary>
public sealed record ContentRestoredEvent(Guid EventId, DateTimeOffset OccurredAt, Guid HoldId, Guid ResourceId, int Sequence) : IIntegrationEvent;

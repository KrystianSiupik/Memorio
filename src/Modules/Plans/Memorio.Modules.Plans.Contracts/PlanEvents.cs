using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Plans.Contracts;

/// <summary>
/// E94: the resource (an account, or an album with <paramref name="IsAlbum"/>) has a paid plan with these limits.
/// Only the effect. <paramref name="Sequence"/> orders events about one resource (D-013).
/// </summary>
public sealed record PlanActivatedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ResourceId, bool IsAlbum, long LimitBytes, int AiLimit, int Sequence) : IIntegrationEvent;

/// <summary>E95: the paid plan ended. Nothing goes; nothing new beyond these limits fits.</summary>
public sealed record PlanReadOnlyEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ResourceId, bool IsAlbum, long LimitBytes, int AiLimit, int Sequence) : IIntegrationEvent;

/// <summary>
/// E97: this many days are left until the trim. <paramref name="Round"/> names the series of warnings: a message
/// is sent once per round, and a delivery report of an earlier round counts for nothing.
/// </summary>
public sealed record TrimWarningDueEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ResourceId, bool IsAlbum, int DaysBefore, DateTimeOffset TrimAt, int Round) : IIntegrationEvent;

/// <summary>E100: the trim waits, because the warning of <paramref name="DaysBefore"/> days has not reached everyone.</summary>
public sealed record TrimHeldEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ResourceId, bool IsAlbum, int DaysBefore, int Round, int Sequence) : IIntegrationEvent;

/// <summary>E103: what is over the free limit goes to the trash for 30 days; the chosen memories are kept first.</summary>
public sealed record TrimOrderedEvent(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid ResourceId,
    bool IsAlbum,
    long LimitBytes,
    int AiLimit,
    IReadOnlyList<Guid> KeptMemoryIds,
    int Sequence) : IIntegrationEvent;

/// <summary>E171: someone paid for the plan of an album without an owner; the album decides who takes it over (D-004).</summary>
public sealed record PaidWhileOwnerlessEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, int Sequence) : IIntegrationEvent;

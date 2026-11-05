using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Notifications.Contracts;

/// <summary>E98: this warning reached everyone it was for: the proof the trim needs (S2:B1). <paramref name="Round"/> as in the warning.</summary>
public sealed record TrimWarningDeliveredEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ResourceId, bool IsAlbum, int DaysBefore, int Round) : IIntegrationEvent;

/// <summary>E99: this warning did not reach someone. Only that it failed, never who or why.</summary>
public sealed record TrimWarningUndeliveredEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ResourceId, bool IsAlbum, int DaysBefore, int Round) : IIntegrationEvent;

using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Plans.Domain.Entitlements;

/// <summary>
/// One line of the entitlement ledger (D-078): the resource is entitled until <see cref="ValidUntil"/>, as
/// <see cref="Source"/> said at <see cref="OccurredAt"/>. Lines are never changed or deleted: a renewal,
/// refund or cancellation is a new line of the same source. <see cref="Source"/> is the stream whose latest
/// word counts (<see cref="EntitlementRules"/>): <see cref="EntitlementSources.Manual"/>, or one subscription
/// at a payment source, named by its adapter. (<see cref="Source"/>, <see cref="SourceReference"/>) is the
/// source's notification, so the same notification recorded twice stays one line.
/// </summary>
internal sealed class EntitlementEntry
{
    // For EF Core when reading the ledger.
    private EntitlementEntry()
    {
        Source = null!;
        SourceReference = null!;
    }

    private EntitlementEntry(
        string source,
        string sourceReference,
        EntitledResource resource,
        DateTimeOffset validUntil,
        DateTimeOffset occurredAt,
        DateTimeOffset recordedAt,
        string? note)
    {
        Source = source;
        SourceReference = sourceReference;
        ResourceKind = resource.Kind;
        ResourceId = resource.Id;
        ValidUntil = validUntil;
        OccurredAt = occurredAt;
        RecordedAt = recordedAt;
        Note = note;
    }

    public string Source { get; private set; }

    public string SourceReference { get; private set; }

    public EntitledResourceKind ResourceKind { get; private set; }

    public Guid ResourceId { get; private set; }

    public EntitledResource Resource => new(ResourceKind, ResourceId);

    /// <summary>The end of the entitlement; at or before <see cref="OccurredAt"/> when the source ended it.</summary>
    public DateTimeOffset ValidUntil { get; private set; }

    /// <summary>When the source decided it (its notification's time), not when it reached us.</summary>
    public DateTimeOffset OccurredAt { get; private set; }

    public DateTimeOffset RecordedAt { get; private set; }

    /// <summary>Why, for a manual line (required) or a dispute; never content of users.</summary>
    public string? Note { get; private set; }

    public static Result<EntitlementEntry> Create(
        string source,
        string sourceReference,
        EntitledResource resource,
        DateTimeOffset validUntil,
        DateTimeOffset occurredAt,
        DateTimeOffset recordedAt,
        string? note = null)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(sourceReference))
        {
            return EntitlementErrors.SourceRequired;
        }

        if (resource.Id == Guid.Empty)
        {
            return EntitlementErrors.ResourceRequired;
        }

        if (source == EntitlementSources.Manual && string.IsNullOrWhiteSpace(note))
        {
            return EntitlementErrors.ReasonRequired;
        }

        return new EntitlementEntry(source, sourceReference, resource, validUntil, occurredAt, recordedAt, note);
    }
}

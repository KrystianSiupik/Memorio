using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Domain.Consents;

/// <summary>
/// S2:C5: whether a person lets the AI use what they brought to one album, for that album only. The owner's consent
/// does not cover the others' content. One per person and album, given and revoked any number of times.
/// </summary>
internal sealed class AiConsent : AggregateRoot<Guid>
{
    private AiConsent(Guid id, Guid accountId, Guid albumId, DateTimeOffset changedAt)
        : base(id)
    {
        AccountId = accountId;
        AlbumId = albumId;
        ChangedAt = changedAt;
    }

    public Guid AccountId { get; private set; }

    public Guid AlbumId { get; private set; }

    public bool IsGiven { get; private set; }

    public DateTimeOffset ChangedAt { get; private set; }

    public int Sequence { get; private set; }

    /// <summary>C80, the first time for this album.</summary>
    public static AiConsent GiveFirst(Guid accountId, Guid albumId, DateTimeOffset now)
    {
        var consent = new AiConsent(Guid.CreateVersion7(), accountId, albumId, now);
        consent.Give(now);
        return consent;
    }

    /// <summary>C80: false when it already was given.</summary>
    public bool Give(DateTimeOffset now)
    {
        if (IsGiven)
        {
            return false;
        }

        IsGiven = true;
        ChangedAt = now;
        Sequence++;
        Raise(new AiConsentGiven(Guid.CreateVersion7(), now, AccountId, AlbumId, Sequence));
        return true;
    }
}

/// <summary>E80.</summary>
internal sealed record AiConsentGiven(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, Guid AlbumId, int Sequence) : IDomainEvent;

internal static class ConsentErrors
{
    public static readonly Error InvalidAlbum = Error.Validation("AiNarration.InvalidAlbum", "The consent needs the album it is for.");

    public static readonly Error NotMember = Error.Forbidden("AiNarration.NotMember", "Only a participant of the album decides on the AI for their content in it.");
}

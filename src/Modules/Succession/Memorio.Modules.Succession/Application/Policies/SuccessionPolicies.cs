using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Succession.Contracts;
using Memorio.Modules.Succession.Domain.Cases;
using Memorio.SharedKernel.Messaging;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Succession.Application.Policies;

/// <summary>
/// P21 and P23 (C125): whenever a death is confirmed, every album of the person goes to its successor's decision and
/// the private archive to the contact, if one was named. Repeating finds what is done and skips it.
/// </summary>
internal sealed class ActOnConfirmedDeath(ITakeoverCases cases, IAlbumSuccession albums, TimeProvider clock) : IIntegrationEventPolicy<DeathConfirmedEvent>
{
    public async Task HandleAsync(DeathConfirmedEvent integrationEvent, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        foreach (var album in await albums.OwnedByAsync(integrationEvent.OwnerAccountId, cancellationToken))
        {
            if (!await cases.ExistsAsync(album.AlbumId, integrationEvent.OwnerAccountId, cancellationToken))
            {
                cases.Add(TakeoverCase.Open(new TakeoverCaseId(Guid.CreateVersion7()), album.AlbumId, integrationEvent.OwnerAccountId, album.SuccessorAccountId, now));
            }
        }

        (await cases.FindContactAsync(integrationEvent.OwnerAccountId, cancellationToken))?.Share(now);
        if (!await cases.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision got there first; the retry reads it again.");
        }
    }
}

/// <summary>
/// A person deleting the account with a successor named: the album goes to the successor's decision like after a death,
/// with nothing to verify (S2:B3). Without one, Album handles it (P37). The contact goes: this was no death.
/// </summary>
internal sealed class OfferAlbumsWhenAccountDeleted(ITakeoverCases cases, IAlbumSuccession albums, TimeProvider clock) : IIntegrationEventPolicy<AccountDeleted>
{
    public async Task HandleAsync(AccountDeleted integrationEvent, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        foreach (var album in await albums.OwnedByAsync(integrationEvent.AccountId, cancellationToken))
        {
            if (album.SuccessorAccountId is not null && !await cases.ExistsAsync(album.AlbumId, integrationEvent.AccountId, cancellationToken))
            {
                cases.Add(TakeoverCase.Open(new TakeoverCaseId(Guid.CreateVersion7()), album.AlbumId, integrationEvent.AccountId, album.SuccessorAccountId, now));
            }
        }

        if (await cases.FindContactAsync(integrationEvent.AccountId, cancellationToken) is { } contact && contact.SharedAt is null)
        {
            cases.Remove(contact);
        }

        if (!await cases.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision got there first; the retry reads it again.");
        }
    }
}

/// <summary>The offer starts its 30 days; a repeat replaces the deadline under the same key.</summary>
internal sealed class AnswerDeadline(IDeadlines deadlines) : IIntegrationEventPolicy<TakeoverOfferedEvent>
{
    public const string JobName = "Succession.AnswerDeadline";

    public Task HandleAsync(TakeoverOfferedEvent integrationEvent, CancellationToken cancellationToken) =>
        deadlines.ScheduleAsync(JobName, integrationEvent.CaseId.ToString(), integrationEvent.AnswerBy, null, cancellationToken);
}

/// <summary>D-021: the effect of E123 and E124 in Album; Album finds what is done and changes nothing.</summary>
internal sealed class ApplyCaseToAlbum(IAlbumSuccession albums) : IIntegrationEventPolicy<AlbumTransferredEvent>, IIntegrationEventPolicy<TakeoverRefusedEvent>
{
    public Task HandleAsync(AlbumTransferredEvent integrationEvent, CancellationToken cancellationToken) =>
        albums.TransferAsync(integrationEvent.AlbumId, integrationEvent.FromAccountId, integrationEvent.ToAccountId, cancellationToken);

    public Task HandleAsync(TakeoverRefusedEvent integrationEvent, CancellationToken cancellationToken) =>
        albums.ReleaseOwnerAsync(integrationEvent.AlbumId, integrationEvent.OwnerAccountId, cancellationToken);
}

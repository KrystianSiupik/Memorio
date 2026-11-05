using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.SharedKernel.Messaging;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Album.Application.Policies;

/// <summary>P2: whenever an account is deleted, what the person brought leaves other people's albums, and they leave too.</summary>
internal sealed class WithdrawContributionsWhenAccountDeleted(IAlbums albums, ContributionWithdrawal withdrawal, TimeProvider clock)
    : IIntegrationEventPolicy<AccountDeleted>
{
    public async Task HandleAsync(AccountDeleted integrationEvent, CancellationToken cancellationToken)
    {
        var applied = await albums.FindAppliedAsync(integrationEvent.AccountId, cancellationToken);
        if (applied?.Sequence > integrationEvent.Sequence)
        {
            return;
        }

        var now = clock.GetUtcNow();
        foreach (var albumId in await albums.FindOtherPeoplesAlbumsAsync(integrationEvent.AccountId, cancellationToken))
        {
            var album = await albums.FindAsync(albumId, cancellationToken);
            if (album is null)
            {
                continue;
            }

            await withdrawal.WithdrawAsync(album, integrationEvent.AccountId, now, cancellationToken);
            album.Leave(integrationEvent.AccountId, now);
        }

        await albums.ForgetInvitationQuotaAsync(integrationEvent.AccountId, cancellationToken);

        if (applied is null)
        {
            albums.Add(new AppliedAccountEvent(integrationEvent.AccountId, integrationEvent.Sequence));
        }
        else
        {
            applied.MoveTo(integrationEvent.Sequence);
        }

        if (!await albums.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about these albums got there first; the retry reads them again.");
        }
    }
}

/// <summary>A memory gone for good leaves its albums, and its space comes back to them.</summary>
internal sealed class ForgetContentWhenDeleted(IAlbums albums, IAlbumContent content, IAlbumPool pool) : IIntegrationEventPolicy<ContentDeleted>
{
    public async Task HandleAsync(ContentDeleted integrationEvent, CancellationToken cancellationToken)
    {
        foreach (var memory in await albums.FindPlacesOfMemoryAsync(integrationEvent.MemoryId, cancellationToken))
        {
            await content.UnlinkAsync(memory.MemoryId, memory.AlbumId.Value, cancellationToken);
            await pool.ReleaseAsync(PoolKeys.For(memory.AlbumId.Value, memory.MemoryId), cancellationToken);
            albums.Remove(memory);
        }

        await albums.SaveAsync(cancellationToken);
    }
}

/// <summary>E42 starts the 7 days of E45; a refreshed invitation replaces its deadline under the same key.</summary>
internal sealed class InvitationReminderDeadline(IAlbums albums, IDeadlines deadlines) : IIntegrationEventPolicy<InvitationSentEvent>
{
    public const string JobName = "Album.InvitationReminder";

    public static readonly TimeSpan Wait = TimeSpan.FromDays(7);

    public async Task HandleAsync(InvitationSentEvent integrationEvent, CancellationToken cancellationToken)
    {
        var album = await albums.FindAsync(new AlbumId(integrationEvent.AlbumId), cancellationToken);
        var invitation = album?.FindInvitation(integrationEvent.InvitationId);
        if (invitation?.State != InvitationState.Pending)
        {
            return;
        }

        await deadlines.ScheduleAsync(JobName, integrationEvent.InvitationId.ToString(), invitation.InvitedAt + Wait, null, cancellationToken);
    }
}

/// <summary>The 7 days ended: E45, if the invitation still waits and was not refreshed in the meantime.</summary>
internal sealed class RecordInvitationWeekPassed(IAlbums albums)
{
    public async Task HandleAsync(Guid invitationId, DateTimeOffset at, CancellationToken cancellationToken)
    {
        var album = await albums.FindByInvitationAsync(invitationId, cancellationToken);
        var invitation = album?.FindInvitation(invitationId);
        if (invitation is null || at < invitation.InvitedAt + InvitationReminderDeadline.Wait)
        {
            return;
        }

        album!.MarkWeekPassed(invitationId, at);
        if (!await albums.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this album got there first; the retry reads it again.");
        }
    }
}

using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Album.Infrastructure.Persistence;

internal sealed class Albums(IDbContextOutbox<AlbumDbContext> outbox) : IAlbums
{
    private AlbumDbContext Database => outbox.DbContext;

    public Task<SharedAlbum?> FindAsync(AlbumId id, CancellationToken cancellationToken) =>
        Database.Albums.FirstOrDefaultAsync(album => album.Id == id, cancellationToken);

    public Task<SharedAlbum?> FindByInvitationAsync(Guid invitationId, CancellationToken cancellationToken) =>
        Database.Albums.FirstOrDefaultAsync(album => album.Invitations.Any(invitation => invitation.Id == invitationId), cancellationToken);

    public void Add(SharedAlbum album) => Database.Albums.Add(album);

    public async Task<bool> TryConsumeInvitationAsync(Guid accountId, DateOnly day, CancellationToken cancellationToken)
    {
        await Database.Database.ExecuteSqlAsync(
            $"DELETE FROM album.invitation_quotas WHERE account_id = {accountId} AND day < {day}",
            cancellationToken);
        return await Database.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO album.invitation_quotas (account_id, day, used) VALUES ({accountId}, {day}, 1)
            ON CONFLICT (account_id, day) DO UPDATE SET used = album.invitation_quotas.used + 1
            WHERE album.invitation_quotas.used < {InvitationQuota.DailyLimit}
            """,
            cancellationToken) == 1;
    }

    public Task ForgetInvitationQuotaAsync(Guid accountId, CancellationToken cancellationToken) =>
        Database.Database.ExecuteSqlAsync($"DELETE FROM album.invitation_quotas WHERE account_id = {accountId}", cancellationToken);

    public Task<AlbumMemory?> FindMemoryAsync(AlbumId albumId, Guid memoryId, CancellationToken cancellationToken) =>
        Database.AlbumMemories.FirstOrDefaultAsync(memory => memory.AlbumId == albumId && memory.MemoryId == memoryId, cancellationToken);

    public async Task<IReadOnlyList<AlbumMemory>> FindMemoriesOfAuthorAsync(AlbumId albumId, Guid authorAccountId, CancellationToken cancellationToken) =>
        await Database.AlbumMemories.Where(memory => memory.AlbumId == albumId && memory.AuthorAccountId == authorAccountId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AlbumMemory>> FindPlacesOfMemoryAsync(Guid memoryId, CancellationToken cancellationToken) =>
        await Database.AlbumMemories.Where(memory => memory.MemoryId == memoryId).ToListAsync(cancellationToken);

    public void Add(AlbumMemory memory) => Database.AlbumMemories.Add(memory);

    public void Remove(AlbumMemory memory) => Database.AlbumMemories.Remove(memory);

    public Task<Entrustment?> FindEntrustmentAsync(AlbumId albumId, Guid authorAccountId, CancellationToken cancellationToken) =>
        Database.Entrustments.FirstOrDefaultAsync(item => item.AlbumId == albumId && item.AuthorAccountId == authorAccountId, cancellationToken);

    public void Add(Entrustment entrustment) => Database.Entrustments.Add(entrustment);

    public void Remove(Entrustment entrustment) => Database.Entrustments.Remove(entrustment);

    public Task<KeptContribution?> FindKeptAsync(AlbumId albumId, Guid authorAccountId, CancellationToken cancellationToken) =>
        Database.KeptContributions.FirstOrDefaultAsync(item => item.AlbumId == albumId && item.AuthorAccountId == authorAccountId, cancellationToken);

    public void Add(KeptContribution kept) => Database.KeptContributions.Add(kept);

    public void Remove(KeptContribution kept) => Database.KeptContributions.Remove(kept);

    public async Task<IReadOnlyList<AlbumId>> FindOtherPeoplesAlbumsAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var taking = Database.Albums
            .Where(album => album.OwnerAccountId != accountId && album.Participants.Any(participant => participant.AccountId == accountId))
            .Select(album => album.Id);
        var brought = Database.AlbumMemories
            .Where(memory => memory.AuthorAccountId == accountId)
            .Select(memory => memory.AlbumId)
            .Where(id => Database.Albums.Any(album => album.Id == id && album.OwnerAccountId != accountId));
        return await taking.Union(brought).ToListAsync(cancellationToken);
    }

    public Task<AppliedAccountEvent?> FindAppliedAsync(Guid accountId, CancellationToken cancellationToken) =>
        Database.AppliedAccountEvents.FirstOrDefaultAsync(applied => applied.AccountId == accountId, cancellationToken);

    public void Add(AppliedAccountEvent applied) => Database.AppliedAccountEvents.Add(applied);

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        foreach (var album in Database.ChangeTracker.Entries<SharedAlbum>().Select(entry => entry.Entity).ToList())
        {
            foreach (var domainEvent in album.DomainEvents)
            {
                await PublishAsync(domainEvent);
            }
        }

        try
        {
            await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception is DbUpdateConcurrencyException
            || exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            Database.ChangeTracker.Clear();
            return false;
        }

        foreach (var entry in Database.ChangeTracker.Entries<SharedAlbum>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    private ValueTask PublishAsync(IDomainEvent domainEvent) => domainEvent switch
    {
        InvitationSent sent => outbox.PublishAsync(new InvitationSentEvent(sent.EventId, sent.OccurredAt, sent.AlbumId.Value, sent.InvitationId, sent.Sequence)),
        InvitationWeekPassed passed => outbox.PublishAsync(new InvitationWeekPassedEvent(passed.EventId, passed.OccurredAt, passed.AlbumId.Value, passed.InvitationId, passed.Sequence)),
        InvitationAccepted accepted => outbox.PublishAsync(new InvitationAcceptedEvent(accepted.EventId, accepted.OccurredAt, accepted.AlbumId.Value, accepted.AccountId, accepted.Sequence)),
        AlbumPoolExhausted full => outbox.PublishAsync(new AlbumPoolExhaustedEvent(full.EventId, full.OccurredAt, full.AlbumId.Value, full.OwnerAccountId, full.Sequence)),
        ParticipantLeft left => outbox.PublishAsync(new ParticipantLeftEvent(left.EventId, left.OccurredAt, left.AlbumId.Value, left.AccountId, left.Sequence)),
        ContributionsWithdrawn withdrawn => outbox.PublishAsync(new ContributionsWithdrawnEvent(withdrawn.EventId, withdrawn.OccurredAt, withdrawn.AlbumId.Value, withdrawn.AuthorAccountId, withdrawn.Sequence)),
        ParticipantRemoved removed => outbox.PublishAsync(new ParticipantRemovedEvent(removed.EventId, removed.OccurredAt, removed.AlbumId.Value, removed.AccountId, removed.Sequence)),
        ParticipantTagged tagged => outbox.PublishAsync(new ParticipantTaggedEvent(tagged.EventId, tagged.OccurredAt, tagged.AlbumId.Value, tagged.MemoryId, tagged.AccountId, tagged.Sequence)),

        // The others stay inside the module.
        _ => ValueTask.CompletedTask,
    };
}

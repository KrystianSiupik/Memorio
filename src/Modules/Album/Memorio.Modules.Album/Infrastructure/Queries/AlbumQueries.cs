using Dapper;
using Memorio.Modules.Album.Application.Queries;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Album.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Album.Infrastructure.Queries;

internal sealed class AlbumQueries(AlbumDbContext database) : IAlbumQueries
{
    private const string MyAlbumsSql = """
        SELECT album.id AS "Id", album.name AS "Name", album.owner_account_id AS "OwnerAccountId",
               (CASE WHEN album.owner_account_id = @noOwner THEN 0 ELSE 1 END) + (SELECT count(*)::int FROM album.album_participants participant
                    WHERE participant.album_id = album.id AND participant.account_id <> album.owner_account_id) AS "Members",
               (SELECT count(*)::int FROM album.album_memories memory WHERE memory.album_id = album.id) AS "Memories"
        FROM album.albums album
        WHERE album.owner_account_id = @accountId
           OR EXISTS (SELECT 1 FROM album.album_participants participant WHERE participant.album_id = album.id AND participant.account_id = @accountId)
        ORDER BY album.created_at DESC, album.id
        """;

    private const string ViewSql = """
        SELECT id AS "Id", name AS "Name", owner_account_id AS "OwnerAccountId", successor_account_id AS "SuccessorAccountId"
        FROM album.albums WHERE id = @albumId
        """;

    private const string MembersSql = """
        SELECT owner_account_id AS "AccountId", true AS "IsOwner" FROM album.albums WHERE id = @albumId AND owner_account_id <> @noOwner
        UNION ALL
        SELECT participant.account_id, false FROM album.album_participants participant
        JOIN album.albums album ON album.id = participant.album_id
        WHERE participant.album_id = @albumId AND participant.account_id <> album.owner_account_id
        """;

    private const string MemoriesSql = """
        SELECT memory.memory_id AS "MemoryId", memory.author_account_id AS "AuthorAccountId", memory.position AS "Position",
               COALESCE((SELECT array_agg(tag.account_id) FROM album.album_memory_tags tag
                         WHERE tag.album_id = memory.album_id AND tag.memory_id = memory.memory_id), '{}'::uuid[]) AS "TaggedAccountIds"
        FROM album.album_memories memory
        WHERE memory.album_id = @albumId
        ORDER BY memory.position NULLS LAST, memory.added_at DESC, memory.memory_id
        """;

    private const string SeatsSql = """
        SELECT (SELECT count(*)::int FROM album.album_participants participant WHERE participant.album_id = album.id AND participant.account_id <> album.owner_account_id)
        FROM album.albums album WHERE album.id = @albumId AND album.owner_account_id = @ownerAccountId
        """;

    private const string InvitedSql = """
        SELECT id AS "Id", email AS "Email" FROM album.album_invitations WHERE album_id = @albumId AND state = 'Pending' ORDER BY invited_at, id
        """;

    private const string InvitationsSql = """
        SELECT invitation.id AS "Id", album.id AS "AlbumId", album.name AS "AlbumName", invitation.invited_by_account_id AS "InvitedByAccountId"
        FROM album.album_invitations invitation
        JOIN album.albums album ON album.id = invitation.album_id
        WHERE invitation.email = @email AND invitation.state = 'Pending'
        ORDER BY invitation.invited_at DESC, invitation.id
        """;

    private const string KeptSql = """
        SELECT kept.album_id AS "AlbumId", album.name AS "AlbumName", kept.chosen_by_author AS "ChosenByAuthor",
               (SELECT count(*)::int FROM album.album_memories memory WHERE memory.album_id = kept.album_id AND memory.author_account_id = kept.author_account_id) AS "Memories"
        FROM album.album_kept_contributions kept
        JOIN album.albums album ON album.id = kept.album_id
        WHERE kept.author_account_id = @accountId
        ORDER BY kept.kept_at DESC, kept.album_id
        """;

    private const string MyEntrustmentSql = """
        SELECT album_id AS "AlbumId", curator_account_id AS "CuratorAccountId"
        FROM album.album_entrustments WHERE album_id = @albumId AND author_account_id = @accountId
        """;

    private const string EntrustedSql = """
        SELECT memory.album_id AS "AlbumId", album.name AS "AlbumName", memory.memory_id AS "MemoryId",
               memory.author_account_id AS "AuthorAccountId", memory.position AS "Position"
        FROM album.album_entrustments entrustment
        JOIN album.album_memories memory ON memory.album_id = entrustment.album_id AND memory.author_account_id = entrustment.author_account_id
        JOIN album.albums album ON album.id = memory.album_id
        WHERE entrustment.curator_account_id = @curatorAccountId
        ORDER BY album.name, memory.position NULLS LAST, memory.added_at DESC, memory.memory_id
        """;

    public async Task<IReadOnlyList<AlbumListItem>> MyAlbumsAsync(Guid accountId, CancellationToken cancellationToken) =>
        [.. await Connection.QueryAsync<AlbumListItem>(new CommandDefinition(MyAlbumsSql, new { accountId, noOwner = SharedAlbum.NoOwner }, cancellationToken: cancellationToken))];

    public async Task<TakeoverState?> TakeoverStateAsync(Guid albumId, CancellationToken cancellationToken)
    {
        var row = await Connection.QuerySingleOrDefaultAsync<TakeoverRow>(new CommandDefinition(
            """
            SELECT owner_account_id = @noOwner AS "IsOwnerless", reserved_by_account_id AS "ReservedByAccountId", reserved_until AS "ReservedUntil"
            FROM album.albums WHERE id = @albumId
            """,
            new { albumId, noOwner = SharedAlbum.NoOwner },
            cancellationToken: cancellationToken));
        return row is null
            ? null
            : new TakeoverState(row.IsOwnerless, row.ReservedByAccountId, row.ReservedUntil is { } until ? new DateTimeOffset(DateTime.SpecifyKind(until, DateTimeKind.Utc)) : null);
    }

    public async Task<HandoverState?> HandoverStateAsync(Guid albumId, CancellationToken cancellationToken)
    {
        var row = await Connection.QuerySingleOrDefaultAsync<HandoverRow>(new CommandDefinition(
            """
            SELECT owner_account_id AS "OwnerAccountId", handover_to_account_id AS "ToAccountId",
                   coalesce(handover_keeps_contributions, true) AS "KeepsContributions", handover_until AS "Until"
            FROM album.albums WHERE id = @albumId
            """,
            new { albumId },
            cancellationToken: cancellationToken));
        return row is null
            ? null
            : new HandoverState(row.OwnerAccountId, row.ToAccountId, row.KeepsContributions, row.Until is { } until ? new DateTimeOffset(DateTime.SpecifyKind(until, DateTimeKind.Utc)) : null);
    }

    public async Task<AlbumView?> ViewAsync(Guid albumId, CancellationToken cancellationToken)
    {
        var album = await Connection.QuerySingleOrDefaultAsync<AlbumRow>(new CommandDefinition(ViewSql, new { albumId }, cancellationToken: cancellationToken));
        if (album is null)
        {
            return null;
        }

        var members = await Connection.QueryAsync<AlbumMemberRow>(new CommandDefinition(MembersSql, new { albumId, noOwner = SharedAlbum.NoOwner }, cancellationToken: cancellationToken));
        var memories = await Connection.QueryAsync<AlbumMemoryRow>(new CommandDefinition(MemoriesSql, new { albumId }, cancellationToken: cancellationToken));
        return new AlbumView(album.Id, album.Name, album.OwnerAccountId, album.SuccessorAccountId, [.. members], [.. memories]);
    }

    public async Task<(int Participants, IReadOnlyList<InvitedRow> Invited)?> SeatsAsync(Guid albumId, Guid ownerAccountId, CancellationToken cancellationToken)
    {
        var participants = await Connection.QuerySingleOrDefaultAsync<int?>(new CommandDefinition(SeatsSql, new { albumId, ownerAccountId }, cancellationToken: cancellationToken));
        if (participants is null)
        {
            return null;
        }

        var invited = await Connection.QueryAsync<InvitedRow>(new CommandDefinition(InvitedSql, new { albumId }, cancellationToken: cancellationToken));
        return (participants.Value, [.. invited]);
    }

    public async Task<IReadOnlyList<InvitationView>> PendingInvitationsAsync(string email, CancellationToken cancellationToken) =>
        [.. await Connection.QueryAsync<InvitationView>(new CommandDefinition(InvitationsSql, new { email }, cancellationToken: cancellationToken))];

    public async Task<IReadOnlyList<KeptRow>> KeptContributionsAsync(Guid accountId, CancellationToken cancellationToken) =>
        [.. await Connection.QueryAsync<KeptRow>(new CommandDefinition(KeptSql, new { accountId }, cancellationToken: cancellationToken))];

    public Task<MyEntrustment?> MyEntrustmentAsync(Guid albumId, Guid accountId, CancellationToken cancellationToken) =>
        Connection.QuerySingleOrDefaultAsync<MyEntrustment>(new CommandDefinition(MyEntrustmentSql, new { albumId, accountId }, cancellationToken: cancellationToken));

    public async Task<IReadOnlyList<EntrustedRow>> EntrustedToAsync(Guid curatorAccountId, CancellationToken cancellationToken) =>
        [.. await Connection.QueryAsync<EntrustedRow>(new CommandDefinition(EntrustedSql, new { curatorAccountId }, cancellationToken: cancellationToken))];

    private System.Data.Common.DbConnection Connection => database.Database.GetDbConnection();

    private sealed record AlbumRow(Guid Id, string Name, Guid OwnerAccountId, Guid? SuccessorAccountId);

    // Dapper reads timestamptz as DateTime.
    private sealed class HandoverRow
    {
        public Guid OwnerAccountId { get; set; }

        public Guid? ToAccountId { get; set; }

        public bool KeepsContributions { get; set; }

        public DateTime? Until { get; set; }
    }

    // Dapper reads timestamptz as DateTime.
    private sealed class TakeoverRow
    {
        public bool IsOwnerless { get; set; }

        public Guid? ReservedByAccountId { get; set; }

        public DateTime? ReservedUntil { get; set; }
    }
}

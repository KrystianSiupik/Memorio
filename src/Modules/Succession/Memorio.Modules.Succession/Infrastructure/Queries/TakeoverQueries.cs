using Dapper;
using Memorio.Modules.Succession.Application.Queries;
using Memorio.Modules.Succession.Contracts;
using Memorio.Modules.Succession.Domain.Cases;
using Memorio.Modules.Succession.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Succession.Infrastructure.Queries;

internal sealed class TakeoverQueries(SuccessionDbContext database) : ITakeoverQueries, ITakeoverOffers, ILegacyContacts
{
    private const string Columns = """
        SELECT id AS "Id", album_id AS "AlbumId", owner_account_id AS "OwnerAccountId", successor_account_id AS "SuccessorAccountId",
               state AS "State", answer_by AS "AnswerBy"
        FROM succession.takeover_cases
        """;

    public async Task<TakeoverOfferRow?> FindAsync(Guid caseId, CancellationToken cancellationToken)
    {
        var row = await Connection.QuerySingleOrDefaultAsync<Row>(new CommandDefinition($"{Columns} WHERE id = @caseId", new { caseId }, cancellationToken: cancellationToken));
        return row?.ToOffer();
    }

    public async Task<IReadOnlyList<TakeoverOfferRow>> OpenOffersOfAsync(Guid accountId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var rows = await Connection.QueryAsync<Row>(new CommandDefinition(
            $"{Columns} WHERE successor_account_id = @accountId AND state = 'Offered' AND answer_by > @now ORDER BY answer_by, id",
            new { accountId, now },
            cancellationToken: cancellationToken));
        return [.. rows.Select(row => row.ToOffer())];
    }

    public async Task<OpenTakeoverOffer?> FindOpenAsync(Guid caseId, CancellationToken cancellationToken)
    {
        var row = await FindAsync(caseId, cancellationToken);
        return row is { State: nameof(CaseState.Offered), SuccessorAccountId: { } successor, AnswerBy: { } answerBy }
            ? new OpenTakeoverOffer(row.AlbumId, successor, answerBy)
            : null;
    }

    public Task<string?> ContactOfAsync(Guid ownerAccountId, CancellationToken cancellationToken) =>
        Connection.ExecuteScalarAsync<string?>(new CommandDefinition(
            "SELECT email FROM succession.legacy_contacts WHERE owner_account_id = @ownerAccountId",
            new { ownerAccountId },
            cancellationToken: cancellationToken));

    private System.Data.Common.DbConnection Connection => database.Database.GetDbConnection();

    // Dapper reads timestamptz as DateTime.
    private sealed class Row
    {
        public Guid Id { get; set; }

        public Guid AlbumId { get; set; }

        public Guid OwnerAccountId { get; set; }

        public Guid? SuccessorAccountId { get; set; }

        public string State { get; set; } = "";

        public DateTime? AnswerBy { get; set; }

        public TakeoverOfferRow ToOffer() => new(
            Id,
            AlbumId,
            OwnerAccountId,
            SuccessorAccountId,
            State,
            AnswerBy is { } at ? new DateTimeOffset(DateTime.SpecifyKind(at, DateTimeKind.Utc)) : null);
    }
}

using Dapper;
using Memorio.Modules.Access.Domain.Audiences;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Access.Infrastructure.Persistence;

/// <summary>One statement, so two deliveries at once leave the later sequence (D-013). A release keeps the kind and the author it found.</summary>
internal sealed class HiddenScopes(AccessDbContext database) : IHiddenScopes
{
    private const string Sql = """
        INSERT INTO access.hidden_scopes (hold_id, resource_kind, resource_id, author_account_id, hidden, sequence, changed_at)
        VALUES (@HoldId, @ResourceKind, @ResourceId, @AuthorAccountId, @Hidden, @Sequence, @ChangedAt)
        ON CONFLICT (hold_id) DO UPDATE SET
            resource_kind = CASE WHEN excluded.resource_kind = '' THEN hidden_scopes.resource_kind ELSE excluded.resource_kind END,
            author_account_id = coalesce(excluded.author_account_id, hidden_scopes.author_account_id),
            hidden = excluded.hidden, sequence = excluded.sequence, changed_at = excluded.changed_at
        WHERE hidden_scopes.sequence < excluded.sequence
        """;

    public async Task<bool> ApplyAsync(HiddenScope scope, CancellationToken cancellationToken)
    {
        await database.Database.GetDbConnection().ExecuteAsync(new CommandDefinition(Sql, scope, cancellationToken: cancellationToken));
        return true;
    }
}

using Dapper;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Account.Infrastructure.Queries;

internal sealed class AccountDirectory(AccountDbContext database) : IAccountDirectory
{
    public Task<string?> EmailOfAsync(Guid accountId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().ExecuteScalarAsync<string?>(new CommandDefinition(
            """SELECT "Email" FROM account."AspNetUsers" WHERE "Id" = @accountId""",
            new { accountId },
            cancellationToken: cancellationToken));

    public async Task<IReadOnlyDictionary<Guid, string>> EmailsOfAsync(IReadOnlyCollection<Guid> accountIds, CancellationToken cancellationToken)
    {
        var rows = await database.Database.GetDbConnection().QueryAsync<(Guid Id, string Email)>(new CommandDefinition(
            """SELECT "Id", "Email" FROM account."AspNetUsers" WHERE "Id" = ANY(@accountIds)""",
            new { accountIds = accountIds.ToArray() },
            cancellationToken: cancellationToken));
        return rows.ToDictionary(row => row.Id, row => row.Email);
    }
}

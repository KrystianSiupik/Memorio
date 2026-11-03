using Dapper;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Account.Infrastructure.Queries;

internal sealed class TermsReader(AccountDbContext database) : ITermsReader
{
    private const string Sql = """
        SELECT EXISTS (SELECT 1 FROM account."AspNetUsers" WHERE "Id" = @accountId AND "AcceptedTermsVersion" = @version)
        """;

    public Task<bool> HasAcceptedCurrentTermsAsync(Guid accountId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().ExecuteScalarAsync<bool>(new CommandDefinition(
            Sql,
            new { accountId, version = TermsOfService.CurrentVersion },
            cancellationToken: cancellationToken));
}

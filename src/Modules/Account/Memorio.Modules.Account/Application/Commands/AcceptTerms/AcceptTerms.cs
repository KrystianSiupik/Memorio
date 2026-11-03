using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Application.Commands.AcceptTerms;

/// <summary>C2: a person accepts the version of the terms they were shown.</summary>
internal sealed record AcceptTerms(AccountId AccountId, string? Version);

/// <summary>Only the current version can be accepted, so the app cannot record terms nobody offers any more.</summary>
internal sealed class AcceptTermsHandler(IAccountRegistry accounts, TimeProvider clock)
{
    public async Task<Result<TermsAccepted>> HandleAsync(AcceptTerms command)
    {
        if (command.Version != TermsOfService.CurrentVersion)
        {
            return AccountErrors.TermsVersionNotCurrent;
        }

        var now = clock.GetUtcNow();
        var recorded = await accounts.RecordTermsAcceptanceAsync(command.AccountId, command.Version, now);
        if (recorded.IsFailure)
        {
            return recorded.Error;
        }

        return new TermsAccepted(Guid.CreateVersion7(), now, command.AccountId, command.Version);
    }
}

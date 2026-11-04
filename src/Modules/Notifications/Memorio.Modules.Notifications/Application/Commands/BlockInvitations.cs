using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Notifications.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Notifications.Application.Commands;

/// <summary>C46: the person's own address takes no more invitations; asking again changes nothing.</summary>
internal sealed class BlockInvitationsHandler(INotificationLedger ledger, IAccountDirectory directory, TimeProvider clock)
{
    public static readonly Error UnknownAccount = Error.Forbidden("Notifications.UnknownAccount", "This account has no address.");

    public async Task<Result> HandleAsync(Guid accountId, CancellationToken cancellationToken)
    {
        if (await directory.EmailOfAsync(accountId, cancellationToken) is not { } email)
        {
            return UnknownAccount;
        }

        await ledger.BlockAsync(email, clock.GetUtcNow(), cancellationToken);
        return Result.Success();
    }
}

using System.Globalization;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.SharedKernel.Messaging;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Account.Application.Policies;

/// <summary>
/// The deadline follows the window the account has now, whatever order the events arrive in: an event about
/// an earlier window finds another sequence or state and changes nothing (D-013). Setting it again replaces it.
/// </summary>
internal sealed class AccountDeletionWindowDeadline(IAccountDeletions deletions, IDeadlines deadlines)
    : IIntegrationEventPolicy<AccountDeletionRequested>, IIntegrationEventPolicy<AccountDeletionCancelled>
{
    public const string JobName = "Account.DeletionWindow";
    public const string SequenceKey = "sequence";

    public async Task HandleAsync(AccountDeletionRequested integrationEvent, CancellationToken cancellationToken)
    {
        var deletion = await deletions.FindAsync(new AccountId(integrationEvent.AccountId), cancellationToken);
        if (deletion?.State != DeletionState.Pending || deletion.Sequence != integrationEvent.Sequence)
        {
            return;
        }

        await deadlines.ScheduleAsync(
            JobName,
            integrationEvent.AccountId.ToString(),
            deletion.WindowEndsAt,
            new Dictionary<string, string> { [SequenceKey] = deletion.Sequence.ToString(CultureInfo.InvariantCulture) },
            cancellationToken);
    }

    public async Task HandleAsync(AccountDeletionCancelled integrationEvent, CancellationToken cancellationToken)
    {
        var deletion = await deletions.FindAsync(new AccountId(integrationEvent.AccountId), cancellationToken);
        if (deletion?.State == DeletionState.Pending)
        {
            return;
        }

        await deadlines.CancelAsync(JobName, integrationEvent.AccountId.ToString(), cancellationToken);
    }
}

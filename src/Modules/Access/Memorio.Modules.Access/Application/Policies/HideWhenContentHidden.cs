using Memorio.Modules.Access.Application.Commands;
using Memorio.Modules.Moderation.Contracts;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Access.Application.Policies;

/// <summary>P32: whenever Moderation hid content, nobody but its author sees it; when the hold ends, it is seen again. Twice or late changes nothing (D-013).</summary>
internal sealed class HideWhenContentHidden(ScopeHandler scopes) : IIntegrationEventPolicy<ContentHiddenEvent>, IIntegrationEventPolicy<ContentRestoredEvent>
{
    public async Task HandleAsync(ContentHiddenEvent integrationEvent, CancellationToken cancellationToken)
    {
        var hidden = await scopes.HideAsync(
            integrationEvent.HoldId, integrationEvent.ResourceKind, integrationEvent.ResourceId, integrationEvent.AuthorAccountId, integrationEvent.Sequence, cancellationToken);
        if (hidden.IsFailure && hidden.Error.Code != "Access.AlreadyDecided")
        {
            throw new InvalidOperationException(hidden.Error.Code);
        }
    }

    public async Task HandleAsync(ContentRestoredEvent integrationEvent, CancellationToken cancellationToken)
    {
        var released = await scopes.ReleaseAsync(integrationEvent.HoldId, integrationEvent.ResourceId, integrationEvent.Sequence, cancellationToken);
        if (released.IsFailure && released.Error.Code != "Access.AlreadyDecided")
        {
            throw new InvalidOperationException(released.Error.Code);
        }
    }
}

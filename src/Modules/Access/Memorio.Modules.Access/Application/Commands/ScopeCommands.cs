using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Access.Domain.Audiences;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Access.Application.Commands;

/// <summary>
/// C67 and its end, the one way a hidden scope changes: the write port of Moderation and the policies on its events
/// both come here, so an order applied at once and its event later are one change (MEM-E8-DOSTEP-02, P32).
/// </summary>
internal sealed class ScopeHandler(IHiddenScopes scopes, TimeProvider clock) : IContentHiding
{
    public async Task<Result> HideAsync(Guid holdId, string? resourceKind, Guid resourceId, Guid? authorAccountId, int sequence, CancellationToken cancellationToken)
    {
        var scope = HiddenScope.Hide(holdId, resourceKind, resourceId, authorAccountId, sequence, clock.GetUtcNow());
        return scope.IsFailure ? scope.Error : await ApplyAsync(scope.Value, cancellationToken);
    }

    public async Task<Result> ReleaseAsync(Guid holdId, Guid resourceId, int sequence, CancellationToken cancellationToken)
    {
        var scope = HiddenScope.Release(holdId, resourceId, sequence, clock.GetUtcNow());
        return scope.IsFailure ? scope.Error : await ApplyAsync(scope.Value, cancellationToken);
    }

    async Task<bool> IContentHiding.HideAsync(HiddenContent content, CancellationToken cancellationToken) =>
        (await HideAsync(content.HoldId, content.ResourceKind, content.ResourceId, content.AuthorAccountId, content.Sequence, cancellationToken)).IsSuccess;

    async Task<bool> IContentHiding.ReleaseAsync(Guid holdId, Guid resourceId, int sequence, CancellationToken cancellationToken) =>
        (await ReleaseAsync(holdId, resourceId, sequence, cancellationToken)).IsSuccess;

    private async Task<Result> ApplyAsync(HiddenScope scope, CancellationToken cancellationToken) =>
        await scopes.ApplyAsync(scope, cancellationToken) ? Result.Success() : AccessErrors.AlreadyDecided;
}

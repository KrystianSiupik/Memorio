using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Plans.Application.Commands.Cycle;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Plans.Application.Policies;

/// <summary>P36: whenever an album has no owner, its plan goes read-only after what is paid ends (D-001, HS-020).</summary>
internal sealed class MarkOwnerlessWhenAlbumLostOwner(MarkAlbumOwnerlessHandler mark) : IIntegrationEventPolicy<AlbumWithoutOwnerEvent>
{
    public async Task HandleAsync(AlbumWithoutOwnerEvent integrationEvent, CancellationToken cancellationToken)
    {
        var marked = await mark.HandleAsync(integrationEvent.AlbumId, integrationEvent.Sequence, cancellationToken);
        if (marked.IsFailure)
        {
            throw new InvalidOperationException(marked.Error.Code);
        }
    }
}

/// <summary>P41: whenever a participant took an album over, its plan is active. They paid for it, which is how it happened (D-004).</summary>
internal sealed class ActivateWhenAlbumTakenOver(ActivateAfterTakeoverHandler activate) : IIntegrationEventPolicy<AlbumTakenOverEvent>
{
    public async Task HandleAsync(AlbumTakenOverEvent integrationEvent, CancellationToken cancellationToken)
    {
        var activated = await activate.HandleAsync(integrationEvent.AlbumId, integrationEvent.Sequence, cancellationToken);
        if (activated.IsFailure && activated.Error.Code != "Plans.NotPaid")
        {
            throw new InvalidOperationException(activated.Error.Code);
        }
    }
}

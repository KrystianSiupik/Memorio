using Memorio.Modules.AiNarration.Application.Policies;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Provenance.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.AiNarration.Infrastructure.Messaging;

/// <summary>E55 in the worker: only resolves this module's policy and calls it (D-135).</summary>
public static class ParticipantLeftHandler
{
    public static Task Handle(ParticipantLeftEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<ConsentEndsWithParticipation>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>A participant removed, in the worker.</summary>
public static class ParticipantRemovedHandler
{
    public static Task Handle(ParticipantRemovedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<ConsentEndsWithParticipation>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E61 from Provenance, in the worker.</summary>
public static class ArtifactInvalidatedHandler
{
    public static Task Handle(ArtifactInvalidatedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<SummaryEndsWithArtifact>().HandleAsync(integrationEvent, cancellationToken);
}

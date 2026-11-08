using Memorio.Modules.Provenance.Domain.Artifacts;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-02: the artifact itself, without a database.</summary>
public sealed class ArtifactTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

    private static ArtifactSource Source(Guid? memory = null) => new(memory ?? Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());

    [Fact]
    public void Registered_it_is_valid_with_its_sources_once_each()
    {
        var memory = Guid.CreateVersion7();

        var artifact = Artifact.Register(Guid.CreateVersion7(), " PeriodSummary ", Guid.CreateVersion7(), Guid.CreateVersion7(), [Source(memory), Source(memory), Source()], Now).Value;

        Assert.Equal(ArtifactState.Valid, artifact.State);
        Assert.Equal("PeriodSummary", artifact.Kind);
        Assert.Equal(2, artifact.Sources.Count);
        Assert.IsType<ArtifactRegistered>(Assert.Single(artifact.DomainEvents));
    }

    [Fact]
    public void Wrong_input_is_refused()
    {
        var owner = Guid.CreateVersion7();

        Assert.Equal(ArtifactErrors.InvalidIds, Artifact.Register(Guid.Empty, "Kind", owner, null, [Source()], Now).Error);
        Assert.Equal(ArtifactErrors.InvalidIds, Artifact.Register(Guid.CreateVersion7(), "Kind", Guid.Empty, null, [Source()], Now).Error);
        Assert.Equal(ArtifactErrors.InvalidKind, Artifact.Register(Guid.CreateVersion7(), " ", owner, null, [Source()], Now).Error);
        Assert.Equal(ArtifactErrors.InvalidKind, Artifact.Register(Guid.CreateVersion7(), new string('k', 51), owner, null, [Source()], Now).Error);
        Assert.Equal(ArtifactErrors.InvalidSources, Artifact.Register(Guid.CreateVersion7(), "Kind", owner, null, [], Now).Error);
        Assert.Equal(ArtifactErrors.InvalidSources, Artifact.Register(Guid.CreateVersion7(), "Kind", owner, null, [new ArtifactSource(Guid.Empty, owner, null)], Now).Error);
        Assert.Equal(
            ArtifactErrors.InvalidSources,
            Artifact.Register(Guid.CreateVersion7(), "Kind", owner, null, [.. Enumerable.Range(0, Artifact.MaxSources + 1).Select(_ => Source())], Now).Error);
    }

    [Fact]
    public void Invalidated_once_it_stays_so()
    {
        var artifact = Artifact.Register(Guid.CreateVersion7(), "PeriodSummary", Guid.CreateVersion7(), null, [Source()], Now).Value;
        artifact.ClearDomainEvents();

        Assert.True(artifact.Invalidate(InvalidationReason.ContentDeleted, Now));
        Assert.False(artifact.Invalidate(InvalidationReason.AccessRevoked, Now.AddDays(1)));

        Assert.Equal(ArtifactState.Invalidated, artifact.State);
        Assert.Equal(InvalidationReason.ContentDeleted, artifact.Reason);
        Assert.Equal(Now, artifact.InvalidatedAt);
        Assert.Equal(2, artifact.Sequence);
        Assert.IsType<ArtifactInvalidated>(Assert.Single(artifact.DomainEvents));
    }
}

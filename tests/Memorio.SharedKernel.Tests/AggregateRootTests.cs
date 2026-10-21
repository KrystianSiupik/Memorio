using Memorio.SharedKernel.Domain;

namespace Memorio.SharedKernel.Tests;

public sealed class AggregateRootTests
{
    [Fact]
    public void Raised_events_are_kept_in_order_until_cleared()
    {
        var aggregate = new Sample(new SampleId(Guid.NewGuid()));

        aggregate.Rename("first");
        aggregate.Rename("second");

        Assert.Equal(["first", "second"], aggregate.DomainEvents.Cast<Renamed>().Select(e => e.Name));

        aggregate.ClearDomainEvents();
        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void Events_cannot_be_added_from_outside()
    {
        var aggregate = new Sample(new SampleId(Guid.NewGuid()));

        Assert.IsNotType<List<IDomainEvent>>(aggregate.DomainEvents, exactMatch: false);
    }

    [Fact]
    public void Entities_are_equal_by_type_and_id()
    {
        var id = new SampleId(Guid.NewGuid());

        Assert.Equal(new Sample(id), new Sample(id));
        Assert.NotEqual(new Sample(id), new Sample(new SampleId(Guid.NewGuid())));
        Assert.NotEqual<object>(new Sample(id), new OtherSample(id));
    }

    private sealed record SampleId(Guid Value);

    private sealed record Renamed(string Name) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();

        public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    }

    private sealed class Sample(SampleId id) : AggregateRoot<SampleId>(id)
    {
        public void Rename(string name) => Raise(new Renamed(name));
    }

    private sealed class OtherSample(SampleId id) : AggregateRoot<SampleId>(id);
}

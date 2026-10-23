using Memorio.Hosting.Messaging;

namespace Memorio.Messaging.Tests;

public sealed class BacklogAgesTests
{
    private static readonly DateTimeOffset Start = new(2026, 10, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Nothing_pending_is_age_zero()
    {
        var ages = new BacklogAges();

        ages.Observe([], Start);

        Assert.Equal(TimeSpan.Zero, ages.OldestAge(Start.AddMinutes(30)));
        Assert.Equal(0, ages.Count);
    }

    [Fact]
    public void A_message_is_as_old_as_the_first_sample_that_saw_it()
    {
        var ages = new BacklogAges();
        Guid stuck = Guid.NewGuid(), later = Guid.NewGuid();

        ages.Observe([stuck], Start);
        ages.Observe([stuck, later], Start.AddMinutes(5));

        Assert.Equal(TimeSpan.FromMinutes(11), ages.OldestAge(Start.AddMinutes(11)));
        Assert.Equal(2, ages.Count);
    }

    [Fact]
    public void A_message_no_longer_pending_stops_counting()
    {
        var ages = new BacklogAges();
        Guid delivered = Guid.NewGuid(), next = Guid.NewGuid();
        ages.Observe([delivered], Start);

        ages.Observe([next], Start.AddMinutes(3));

        Assert.Equal(TimeSpan.FromMinutes(1), ages.OldestAge(Start.AddMinutes(4)));
        Assert.Equal(1, ages.Count);
    }
}

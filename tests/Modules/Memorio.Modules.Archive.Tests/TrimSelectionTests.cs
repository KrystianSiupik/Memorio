using Memorio.Modules.Archive.Domain.Memories;

namespace Memorio.Modules.Archive.Tests;

/// <summary>What stays when an archive comes down to its limit (S2:B1).</summary>
public sealed class TrimSelectionTests
{
    private static readonly Guid Oldest = Guid.CreateVersion7();
    private static readonly Guid Middle = Guid.CreateVersion7();
    private static readonly Guid Newest = Guid.CreateVersion7();

    private static TrimCandidate[] Three(long size = 100) =>
    [
        new(Newest, new DateOnly(2020, 1, 1), size),
        new(Oldest, new DateOnly(1980, 1, 1), size),
        new(Middle, new DateOnly(2000, 1, 1), size),
    ];

    [Fact]
    public void Without_a_choice_the_oldest_stays() =>
        Assert.Equal([Middle, Newest], TrimSelection.ToTrim(Three(), [], 100));

    [Fact]
    public void The_choice_comes_before_the_oldest() =>
        Assert.Equal([Oldest, Middle], TrimSelection.ToTrim(Three(), [Newest], 100));

    [Fact]
    public void A_choice_above_the_limit_keeps_what_fits_from_the_oldest_chosen() =>
        Assert.Equal([Newest, Middle], TrimSelection.ToTrim(Three(), [Newest, Oldest], 100));

    [Fact]
    public void What_does_not_fit_is_passed_over_and_smaller_ones_still_stay()
    {
        TrimCandidate[] candidates =
        [
            new(Oldest, new DateOnly(1980, 1, 1), 500),
            new(Middle, new DateOnly(2000, 1, 1), 50),
            new(Newest, new DateOnly(2020, 1, 1), 50),
        ];

        Assert.Equal([Oldest], TrimSelection.ToTrim(candidates, [], 100));
    }

    [Fact]
    public void Under_the_limit_nothing_goes() =>
        Assert.Empty(TrimSelection.ToTrim(Three(), [], 300));

    [Fact]
    public void A_memory_without_photos_always_fits() =>
        Assert.Empty(TrimSelection.ToTrim(Three(size: 0), [], 0));

    [Fact]
    public void A_chosen_id_that_is_no_candidate_changes_nothing() =>
        Assert.Equal([Middle, Newest], TrimSelection.ToTrim(Three(), [Guid.CreateVersion7()], 100));
}

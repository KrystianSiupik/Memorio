using Memorio.Testing;
using Xunit.Sdk;
using static Memorio.Modules.Access.Tests.ProbeAlbums;

namespace Memorio.Modules.Access.Tests;

/// <summary>
/// MEM-E0-PLAT-14: the access matrix on the probe albums, and proof that it fails on the mistakes it is
/// there to catch: a search without the Access filter, a read without asking Access, a cached answer, an
/// undecided pair. The real rows of the module come with the E3, E4 and E8 tickets.
/// </summary>
public sealed class AccessMatrixTests(AccessDatabase database)
{
    private static readonly AccessMatrix Matrix = new AccessMatrix(Actors, Resources)
        .Sees(OwnerOfA, AlbumA).DoesNotSee(OwnerOfA, AlbumB)
        .Sees(OwnerOfB, AlbumB).DoesNotSee(OwnerOfB, AlbumA)
        .Sees(ParticipantOfA, AlbumA).DoesNotSee(ParticipantOfA, AlbumB);

    public static TheoryData<string, string, bool> Rows => Matrix.Rows();

    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Theory]
    [MemberData(nameof(Rows))]
    public async Task Row(string actor, string resource, bool sees)
    {
        var world = await CreateAsync(database);

        await AccessMatrix.AssertRowAsync(world, actor, resource, sees, Cancellation);
    }

    [Fact]
    public async Task Access_taken_away_is_gone_at_the_next_read_and_search()
    {
        var world = await CreateAsync(database);

        await AccessMatrix.AssertRevokedAsync(world, ParticipantOfA, AlbumA, Cancellation);

        // Only that person lost it.
        await AccessMatrix.AssertRowAsync(world, OwnerOfA, AlbumA, sees: true, Cancellation);
    }

    [Fact]
    public async Task Fails_on_a_search_without_the_Access_filter()
    {
        var world = await CreateAsync(database, mistake: Mistake.SearchWithoutAccess);

        await Assert.ThrowsAnyAsync<XunitException>(
            () => AccessMatrix.AssertRowAsync(world, OwnerOfB, AlbumA, sees: false, Cancellation));
    }

    [Fact]
    public async Task Fails_on_a_read_without_asking_Access()
    {
        var world = await CreateAsync(database, mistake: Mistake.OpenWithoutAccess);

        await Assert.ThrowsAnyAsync<XunitException>(
            () => AccessMatrix.AssertRowAsync(world, OwnerOfB, AlbumA, sees: false, Cancellation));
    }

    [Fact]
    public async Task Fails_when_access_taken_away_lingers_in_a_cache()
    {
        var world = await CreateAsync(database, reader => new CachingAccessReader(reader));

        await Assert.ThrowsAnyAsync<XunitException>(
            () => AccessMatrix.AssertRevokedAsync(world, ParticipantOfA, AlbumA, Cancellation));
    }

    [Fact]
    public void Fails_on_a_pair_nobody_decided()
    {
        var matrix = new AccessMatrix(Actors, Resources).Sees(OwnerOfA, AlbumA);

        var error = Assert.Throws<InvalidOperationException>(() => matrix.Rows());
        Assert.Contains($"{OwnerOfB} × {AlbumA}", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Fails_on_a_pair_decided_twice_or_an_unknown_name()
    {
        Assert.Throws<InvalidOperationException>(
            () => new AccessMatrix(Actors, Resources).Sees(OwnerOfA, AlbumA).DoesNotSee(OwnerOfA, AlbumA));
        Assert.Throws<InvalidOperationException>(() => new AccessMatrix(Actors, Resources).Sees("stranger", AlbumA));
    }
}

using Xunit;

namespace Memorio.Testing;

/// <summary>
/// The access matrix of MEM-E0-PLAT-14 (T14, ADR 0011, SD11, QS16): for every actor and every resource of
/// a scenario, whether the actor sees it. A module whose queries return someone else's content describes
/// its scenario as an <see cref="IAccessWorld"/> on the real database and its expectations here; each row
/// becomes one test case, checked through both the single read and the list or search, and taking access
/// away is checked on the very next query (C9). How to add rows: src/Modules/README.md.
/// </summary>
public sealed class AccessMatrix
{
    private readonly string[] _actors;
    private readonly string[] _resources;
    private readonly Dictionary<(string Actor, string Resource), bool> _expected = [];

    public AccessMatrix(IEnumerable<string> actors, IEnumerable<string> resources)
    {
        _actors = [.. actors];
        _resources = [.. resources];
    }

    public AccessMatrix Sees(string actor, params string[] resources) => Expect(actor, resources, true);

    public AccessMatrix DoesNotSee(string actor, params string[] resources) => Expect(actor, resources, false);

    /// <summary>
    /// One row per actor × resource, for <c>[MemberData]</c>. Fails when a pair is left undecided, so a new
    /// actor or resource cannot slip in without saying who sees it.
    /// </summary>
    public TheoryData<string, string, bool> Rows()
    {
        var undecided = _actors
            .SelectMany(actor => _resources.Select(resource => (actor, resource)))
            .Where(pair => !_expected.ContainsKey(pair))
            .Select(pair => $"{pair.actor} × {pair.resource}")
            .ToList();
        if (undecided.Count > 0)
        {
            throw new InvalidOperationException($"The access matrix leaves undecided: {string.Join(", ", undecided)}.");
        }

        var rows = new TheoryData<string, string, bool>();
        foreach (var ((actor, resource), sees) in _expected)
        {
            rows.Add(actor, resource, sees);
        }

        return rows;
    }

    /// <summary>The actor sees the resource exactly when expected, in the single read and in the list or search.</summary>
    public static async Task AssertRowAsync(IAccessWorld world, string actor, string resource, bool sees, CancellationToken cancellationToken)
    {
        var opens = await world.OpensAsync(actor, resource, cancellationToken);
        Assert.True(opens == sees, $"{actor} {(sees ? "should open" : "opened")} {resource} (single read).");

        var found = await world.FindsAsync(actor, cancellationToken);
        Assert.True(found.Contains(resource) == sees, $"{actor} {(sees ? "should find" : "found")} {resource} (list or search).");
    }

    /// <summary>
    /// The actor sees the resource, loses access, and the very next single read and list or search no
    /// longer show it: no waiting, no cache (C9, D-031).
    /// </summary>
    public static async Task AssertRevokedAsync(IAccessWorld world, string actor, string resource, CancellationToken cancellationToken)
    {
        await AssertRowAsync(world, actor, resource, sees: true, cancellationToken);

        await world.RevokeAsync(actor, resource, cancellationToken);

        Assert.False(await world.OpensAsync(actor, resource, cancellationToken), $"{actor} still opens {resource} after losing access.");
        Assert.DoesNotContain(resource, await world.FindsAsync(actor, cancellationToken));
    }

    private AccessMatrix Expect(string actor, string[] resources, bool sees)
    {
        Known(actor, _actors, "actor");
        foreach (var resource in resources)
        {
            Known(resource, _resources, "resource");
            if (!_expected.TryAdd((actor, resource), sees))
            {
                throw new InvalidOperationException($"{actor} × {resource} is decided twice in the access matrix.");
            }
        }

        return this;
    }

    private static void Known(string name, string[] names, string kind)
    {
        if (!names.Contains(name, StringComparer.Ordinal))
        {
            throw new InvalidOperationException($"Unknown {kind} '{name}' in the access matrix.");
        }
    }
}

/// <summary>
/// A module's access scenario on the real database: named actors and resources, set up by the test,
/// read the way the module's queries read them (through <c>IAccessReader</c>, ADR 0011).
/// </summary>
public interface IAccessWorld
{
    /// <summary>The single read of the resource as the actor (open an album, sign a media address).</summary>
    Task<bool> OpensAsync(string actor, string resource, CancellationToken cancellationToken);

    /// <summary>The names of the resources a list or search as the actor returns.</summary>
    Task<IReadOnlyCollection<string>> FindsAsync(string actor, CancellationToken cancellationToken);

    /// <summary>Takes the actor's access to the resource away, the way the module's command does.</summary>
    Task RevokeAsync(string actor, string resource, CancellationToken cancellationToken);
}

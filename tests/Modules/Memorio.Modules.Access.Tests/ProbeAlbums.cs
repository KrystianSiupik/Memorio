using Memorio.Modules.Access.Contracts;
using Memorio.Testing;

namespace Memorio.Modules.Access.Tests;

/// <summary>
/// The proof that the access matrix catches what it should, until the real albums and rules exist (E3,
/// E4): two albums and three people on the real database. "owner of A" and "owner of B" have their
/// albums, "participant of A" was let into album A. Grants live in the access schema behind
/// <see cref="IAccessReader"/>; the albums and their queries live in the album schema and ask the reader
/// at every query, the way ADR 0011 and D-031 describe. A <see cref="Mistake"/> breaks it on purpose.
/// </summary>
internal sealed class ProbeAlbums : IAccessWorld
{
    public const string OwnerOfA = "owner of A";
    public const string OwnerOfB = "owner of B";
    public const string ParticipantOfA = "participant of A";
    public const string AlbumA = "album A";
    public const string AlbumB = "album B";

    public static readonly string[] Actors = [OwnerOfA, OwnerOfB, ParticipantOfA];
    public static readonly string[] Resources = [AlbumA, AlbumB];

    private readonly AccessDatabase _database;
    private readonly IAccessReader _reader;
    private readonly Mistake _mistake;
    private readonly Dictionary<string, Guid> _people = Actors.ToDictionary(actor => actor, _ => Guid.NewGuid());
    private readonly Dictionary<string, Guid> _albums = Resources.ToDictionary(album => album, _ => Guid.NewGuid());

    private ProbeAlbums(AccessDatabase database, IAccessReader reader, Mistake mistake)
    {
        _database = database;
        _reader = reader;
        _mistake = mistake;
    }

    public enum Mistake
    {
        None,

        /// <summary>The search returns the albums of the scenario without the Access filter.</summary>
        SearchWithoutAccess,

        /// <summary>Opening an album reads it without asking Access.</summary>
        OpenWithoutAccess,
    }

    /// <param name="reader">The Access read port; <see cref="ProbeAccessReader"/> unless a test breaks it.</param>
    public static async Task<ProbeAlbums> CreateAsync(
        AccessDatabase database,
        Func<IAccessReader, IAccessReader>? reader = null,
        Mistake mistake = Mistake.None)
    {
        var probeReader = new ProbeAccessReader(database);
        var world = new ProbeAlbums(database, reader?.Invoke(probeReader) ?? probeReader, mistake);

        foreach (var (title, id) in world._albums)
        {
            await using var command = database.Album.CreateCommand("INSERT INTO album.probe_albums (id, title) VALUES ($1, $2)");
            command.Parameters.Add(new() { Value = id });
            command.Parameters.Add(new() { Value = title });
            await command.ExecuteNonQueryAsync();
        }

        await world.GrantAsync(OwnerOfA, AlbumA);
        await world.GrantAsync(OwnerOfB, AlbumB);
        await world.GrantAsync(ParticipantOfA, AlbumA);
        return world;
    }

    public async Task<bool> OpensAsync(string actor, string resource, CancellationToken cancellationToken)
    {
        if (_mistake != Mistake.OpenWithoutAccess
            && !await _reader.CanSeeAlbumAsync(_people[actor], _albums[resource], cancellationToken))
        {
            return false;
        }

        await using var command = _database.Album.CreateCommand("SELECT title FROM album.probe_albums WHERE id = $1");
        command.Parameters.Add(new() { Value = _albums[resource] });
        return await command.ExecuteScalarAsync(cancellationToken) is string;
    }

    public async Task<IReadOnlyCollection<string>> FindsAsync(string actor, CancellationToken cancellationToken)
    {
        // The Access filter in the search's own SQL (D-031). Only this scenario's albums: other tests share the table.
        var visible = await _reader.VisibleAlbumsAsync(_people[actor], cancellationToken);
        await using var command = _database.Album.CreateCommand(
            _mistake == Mistake.SearchWithoutAccess
                ? "SELECT title FROM album.probe_albums WHERE id = ANY($1)"
                : "SELECT title FROM album.probe_albums WHERE id = ANY($1) AND id = ANY($2)");
        command.Parameters.Add(new() { Value = _albums.Values.ToArray() });
        if (_mistake != Mistake.SearchWithoutAccess)
        {
            command.Parameters.Add(new() { Value = visible.ToArray() });
        }

        var titles = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            titles.Add(reader.GetString(0));
        }

        return titles;
    }

    public async Task RevokeAsync(string actor, string resource, CancellationToken cancellationToken)
    {
        await using var command = _database.Access.CreateCommand("DELETE FROM access.probe_grants WHERE person_id = $1 AND album_id = $2");
        command.Parameters.Add(new() { Value = _people[actor] });
        command.Parameters.Add(new() { Value = _albums[resource] });
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task GrantAsync(string actor, string album)
    {
        await using var command = _database.Access.CreateCommand("INSERT INTO access.probe_grants (person_id, album_id) VALUES ($1, $2)");
        command.Parameters.Add(new() { Value = _people[actor] });
        command.Parameters.Add(new() { Value = _albums[album] });
        await command.ExecuteNonQueryAsync();
    }
}

/// <summary>A probe Access read port on the grants table, asked fresh every time.</summary>
internal sealed class ProbeAccessReader(AccessDatabase database) : IAccessReader
{
    public async Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken)
    {
        await using var command = database.Access.CreateCommand(
            "SELECT EXISTS (SELECT 1 FROM access.probe_grants WHERE person_id = $1 AND album_id = $2)");
        command.Parameters.Add(new() { Value = personId });
        command.Parameters.Add(new() { Value = albumId });
        return (bool)(await command.ExecuteScalarAsync(cancellationToken))!;
    }

    public async Task<IReadOnlyCollection<Guid>> VisibleAlbumsAsync(Guid personId, CancellationToken cancellationToken)
    {
        await using var command = database.Access.CreateCommand("SELECT album_id FROM access.probe_grants WHERE person_id = $1");
        command.Parameters.Add(new() { Value = personId });
        var albums = new List<Guid>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            albums.Add(reader.GetGuid(0));
        }

        return albums;
    }
}

/// <summary>A broken Access read port: remembers the first answer, so access taken away lingers.</summary>
internal sealed class CachingAccessReader(IAccessReader inner) : IAccessReader
{
    private readonly Dictionary<(Guid, Guid), bool> _canSee = [];
    private readonly Dictionary<Guid, IReadOnlyCollection<Guid>> _visible = [];

    public async Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken)
    {
        if (!_canSee.TryGetValue((personId, albumId), out var canSee))
        {
            _canSee[(personId, albumId)] = canSee = await inner.CanSeeAlbumAsync(personId, albumId, cancellationToken);
        }

        return canSee;
    }

    public async Task<IReadOnlyCollection<Guid>> VisibleAlbumsAsync(Guid personId, CancellationToken cancellationToken)
    {
        if (!_visible.TryGetValue(personId, out var visible))
        {
            _visible[personId] = visible = await inner.VisibleAlbumsAsync(personId, cancellationToken);
        }

        return visible;
    }
}

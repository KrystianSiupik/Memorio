using Npgsql;

namespace Memorio.Hosting.Messaging;

/// <summary>Connections of the role memorio_messaging, kept apart from the modules' own data sources.</summary>
internal sealed class MessagingDatabase(NpgsqlDataSource dataSource) : IAsyncDisposable
{
    public NpgsqlDataSource DataSource { get; } = dataSource;

    public ValueTask DisposeAsync() => DataSource.DisposeAsync();
}

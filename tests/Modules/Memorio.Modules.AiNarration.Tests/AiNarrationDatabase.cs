using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.Modules.AiNarration.Infrastructure.Persistence;
using Memorio.Modules.Limits.Contracts;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: AssemblyFixture(typeof(Memorio.Modules.AiNarration.Tests.AiNarrationDatabase))]

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>
/// The bootstrapped database with the module's migrations applied by its own role (as a host applies them),
/// and the module's services on it, registered by <see cref="AiNarrationModule"/> itself.
/// </summary>
public sealed class AiNarrationDatabase : PostgresDatabase
{
    public const string Role = "memorio_ai_narration";

    private ServiceProvider _services = null!;

    public AsyncServiceScope CreateScope() => _services.CreateAsyncScope();

    internal FakeAllowance Allowance { get; } = new();

    internal FakeProviderForDrafts Provider { get; } = new();

    protected override async Task OnBootstrappedAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AiNarration:ConnectionString"] = ConnectionStringFor(Role),
                ["Database:MigrateOnStartup"] = "false",
            })
            .Build();
        var services = new ServiceCollection();
        new AiNarrationModule().AddServices(services, configuration);
        services.AddSingleton<IAiAllowance>(Allowance);
        services.RemoveAll<ITextGenerator>();
        services.AddSingleton<ITextGenerator>(Provider);
        _services = services.BuildServiceProvider();

        await using var scope = CreateScope();
        await scope.ServiceProvider.GetRequiredService<AiNarrationDbContext>().Database.MigrateAsync();
    }

    protected override async Task OnDisposingAsync() => await _services.DisposeAsync();
}

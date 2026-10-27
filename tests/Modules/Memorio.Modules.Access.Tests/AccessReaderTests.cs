using Memorio.Modules.Access.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access.Tests;

/// <summary>Until the E4 rules exist, the module's read port denies everything (ADR 0011: no rule, no access).</summary>
public sealed class AccessReaderTests
{
    [Fact]
    public async Task Nobody_sees_anything_until_a_rule_grants_it()
    {
        var services = new ServiceCollection();
        new AccessModule().AddServices(services, new ConfigurationBuilder().Build());
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var reader = scope.ServiceProvider.GetRequiredService<IAccessReader>();

        Assert.False(await reader.CanSeeAlbumAsync(Guid.NewGuid(), Guid.NewGuid(), TestContext.Current.CancellationToken));
        Assert.Empty(await reader.VisibleAlbumsAsync(Guid.NewGuid(), TestContext.Current.CancellationToken));
    }
}

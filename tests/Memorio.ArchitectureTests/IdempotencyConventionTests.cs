namespace Memorio.ArchitectureTests;

/// <summary>The SD6 convention itself, on the fixture module Alpha with one message handler.</summary>
public sealed class IdempotencyConventionTests
{
    private const string FixturesRoot = "Memorio.ArchitectureTests.Fixtures.Valid.Modules";

    private static readonly IReadOnlyList<(string Module, Type Handler)> Handlers =
        IdempotencyConvention.HandlersIn(typeof(IdempotencyConventionTests).Assembly.GetTypes(), FixturesRoot);

    [Fact]
    public void Finds_the_public_static_handlers_in_Infrastructure_Messaging()
    {
        Assert.Equal(["Alpha.BetaInfoChangedHandler"], Handlers.Select(handler => $"{handler.Module}.{handler.Handler.Name}"));
    }

    [Fact]
    public void A_handler_with_an_idempotency_test_in_its_module_passes()
    {
        var missing = IdempotencyConvention.HandlersWithoutTest(
            Handlers,
            module => module == "Alpha" ? ["BetaInfoChangedHandlerIdempotencyTests"] : []);

        Assert.Empty(missing);
    }

    [Theory]
    [InlineData("Alpha", "BetaInfoChangedHandlerTests")]
    [InlineData("Beta", "BetaInfoChangedHandlerIdempotencyTests")]
    public void A_handler_without_an_idempotency_test_in_its_own_module_fails(string testModule, string testClass)
    {
        var missing = IdempotencyConvention.HandlersWithoutTest(
            Handlers,
            module => module == testModule ? [testClass] : []);

        Assert.Equal(["Alpha.BetaInfoChangedHandler"], missing);
    }
}

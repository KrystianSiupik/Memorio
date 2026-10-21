using Memorio.SharedKernel.Results;

namespace Memorio.SharedKernel.Tests;

public sealed class ResultTests
{
    private static readonly Error NotFound = Error.NotFound("Album.NotFound", "No album with this id.");

    [Fact]
    public void A_success_carries_its_value_and_has_no_error()
    {
        Result<int> result = 42;

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void A_failure_carries_its_error_and_has_no_value()
    {
        Result<int> result = NotFound;

        Assert.True(result.IsFailure);
        Assert.Equal(NotFound, result.Error);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Match_runs_the_branch_of_the_outcome()
    {
        Assert.Equal("42", Result.Success(42).Match(value => value.ToString(System.Globalization.CultureInfo.InvariantCulture), error => error.Code));
        Assert.Equal("Album.NotFound", Result.Failure<int>(NotFound).Match(value => "unexpected", error => error.Code));
    }

    [Fact]
    public void A_result_without_a_value_is_a_success_or_an_error()
    {
        Assert.True(Result.Success().IsSuccess);

        Result failure = NotFound;
        Assert.Equal(NotFound, failure.Error);
    }

    [Fact]
    public void An_error_needs_a_code_and_a_description()
    {
        Assert.Throws<ArgumentException>(() => Error.Validation(" ", "Description."));
        Assert.Throws<ArgumentException>(() => Error.Validation("Album.Name", ""));
    }

    [Fact]
    public void Each_factory_sets_its_error_type()
    {
        Assert.Equal(ErrorType.Validation, Error.Validation("A.B", "d").Type);
        Assert.Equal(ErrorType.NotFound, Error.NotFound("A.B", "d").Type);
        Assert.Equal(ErrorType.Conflict, Error.Conflict("A.B", "d").Type);
        Assert.Equal(ErrorType.Forbidden, Error.Forbidden("A.B", "d").Type);
        Assert.Equal(ErrorType.Failure, Error.Failure("A.B", "d").Type);
    }
}

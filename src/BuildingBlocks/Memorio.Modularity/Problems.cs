using Memorio.SharedKernel.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Memorio.Modularity;

/// <summary>
/// The API edge of <see cref="Error"/> (D-102): RFC 7807 problem details with the extension "code";
/// the HTTP status comes from <see cref="ErrorType"/> only here.
/// </summary>
public static class Problems
{
    public static ProblemHttpResult From(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return WithCode(
            error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError,
            },
            error.Code);
    }

    /// <summary>A problem with an explicit status, for outcomes that are not business errors (e.g. 401).</summary>
    public static ProblemHttpResult WithCode(int status, string code) =>
        TypedResults.Problem(statusCode: status, extensions: new Dictionary<string, object?> { ["code"] = code });
}

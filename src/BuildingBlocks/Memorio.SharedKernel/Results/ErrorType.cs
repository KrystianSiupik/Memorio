namespace Memorio.SharedKernel.Results;

/// <summary>Kind of business failure; mapped to an HTTP status only at the API edge.</summary>
public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Forbidden,
    Failure,
}

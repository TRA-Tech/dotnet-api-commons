using Microsoft.AspNetCore.Http;

namespace ApiCommons.Result;

/// <summary>
/// Base type for all domain errors. Override <see cref="StatusCode"/>, <see cref="Detail"/>,
/// and <see cref="Code"/> to control how this error is represented in an HTTP response.
/// </summary>
public abstract record Error
{
    /// <summary>The HTTP status code to return. Defaults to <c>500 Internal Server Error</c>.</summary>
    public virtual int StatusCode => StatusCodes.Status500InternalServerError;

    /// <summary>
    /// A human-readable explanation specific to this occurrence of the problem (RFC 9457 <c>detail</c>).
    /// Returns <see langword="null"/> by default, which omits the field from the response.
    /// </summary>
    public virtual string? Detail => null;

    /// <summary>
    /// A machine-readable error code included in the <c>ProblemDetails</c> extensions (e.g. <c>"PRODUCT_NOT_FOUND"</c>).
    /// Returns <see langword="null"/> by default, which omits the field from the response.
    /// </summary>
    public virtual string? Code => null;
}

/// <summary>The requested resource could not be found.</summary>
public record NotFoundError(string Resource) : Error
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string? Detail => $"{Resource} was not found.";
    public override string? Code => $"{Resource.ToUpperInvariant()}_NOT_FOUND";
}

/// <summary>The resource already exists and cannot be created again.</summary>
public record AlreadyExistsError(string Resource) : Error
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string? Detail => $"{Resource} already exists.";
    public override string? Code => $"{Resource.ToUpperInvariant()}_ALREADY_EXISTS";
}

/// <summary>The request conflicts with the current state of the resource.</summary>
public record ConflictError(string Reason) : Error
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string? Detail => Reason;
    public override string? Code => "CONFLICT";
}

/// <summary>The request lacks valid authentication credentials.</summary>
public record UnauthorizedError(string Message) : Error
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string? Detail => Message;
    public override string? Code => "UNAUTHORIZED";
}

/// <summary>The authenticated caller does not have permission to perform this action.</summary>
public record ForbiddenError(string Resource, string Action) : Error
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string? Detail => $"{Action} on {Resource} is forbidden.";
    public override string? Code => $"{Resource.ToUpperInvariant()}_{Action.ToUpperInvariant()}_FORBIDDEN";
}

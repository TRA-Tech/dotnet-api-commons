using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiCommons.Middlewares.GlobalErrorHandler;

/// <summary>
/// Default <see cref="IExceptionHandler"/> implementation that logs all unhandled exceptions
/// and maps them to a 500 Internal Server Error ProblemDetails response.
/// Implement <see cref="IExceptionHandler"/> directly when custom error mapping or logging behaviour is needed.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception for {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred."
            },
            cancellationToken);
        return true;
    }
}

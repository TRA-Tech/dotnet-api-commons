using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Playground.Middlewares;

/// <summary>
/// Demonstrates the v2 IExceptionHandler registration pattern from ApiCommons Phase 5.
/// Logs all unhandled exceptions and returns a 500 ProblemDetails response.
/// </summary>
internal sealed class PlaygroundExceptionHandler(ILogger<PlaygroundExceptionHandler> logger)
    : IExceptionHandler
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

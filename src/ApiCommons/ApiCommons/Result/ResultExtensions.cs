using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCommons.Result;

public static class ResultExtensions
{
    private static IActionResult MapError(Error error)
    {
        var problem = new ProblemDetails { Status = error.StatusCode, Detail = error.Detail };
        if (error.Code is not null)
            problem.Extensions["code"] = error.Code;
        return new ObjectResult(problem) { StatusCode = error.StatusCode };
    }

    // ── Sync ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to an <see cref="IActionResult"/>.
    /// Success → <c>200 OK</c> with the value as the response body.
    /// Failure → <c>ProblemDetails</c> (RFC 9457) with the HTTP status mapped from the <see cref="Error"/> subtype.
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.Match<IActionResult>(
            value => new OkObjectResult(value),
            MapError);

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to an <see cref="IActionResult"/> with a custom success status code.
    /// Success → <paramref name="statusCode"/> with the value as the response body (e.g. <c>201 Created</c>).
    /// Failure → <c>ProblemDetails</c> (RFC 9457) with the HTTP status mapped from the <see cref="Error"/> subtype.
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result, int statusCode) =>
        result.Match<IActionResult>(
            value => new ObjectResult(value) { StatusCode = statusCode },
            MapError);

    /// <summary>
    /// Converts a <see cref="Result{T}"/> of <see cref="Unit"/> to an <see cref="IActionResult"/>.
    /// Success → <c>204 No Content</c>.
    /// Failure → <c>ProblemDetails</c> (RFC 9457) with the HTTP status mapped from the <see cref="Error"/> subtype.
    /// </summary>
    public static IActionResult ToActionResult(this Result<Unit> result) =>
        result.Match<IActionResult>(
            _ => new NoContentResult(),
            MapError);

    // ── Async pipeline ────────────────────────────────────────────────────────

    /// <summary>
    /// Awaits the task, then chains a <see cref="Result{TNew}"/>-returning async operation.
    /// Short-circuits and propagates the failure without invoking <paramref name="binder"/>
    /// if the awaited result is already a failure.
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<TValue, TNew>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<Result<TNew>>> binder)
    {
        var result = await resultTask;
        return result.IsError ? result.Error! : await binder(result.Value!);
    }

    /// <summary>
    /// Awaits the task, then transforms the success value with <paramref name="mapper"/>.
    /// Propagates the failure unchanged if the awaited result is a failure.
    /// </summary>
    public static async Task<Result<TNew>> Map<TValue, TNew>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, TNew> mapper)
    {
        var result = await resultTask;
        return result.Map(mapper);
    }

    /// <summary>
    /// Awaits the task, then invokes <paramref name="action"/> on the success value.
    /// Returns the original result unchanged. The action is not called on failure.
    /// </summary>
    public static async Task<Result<TValue>> Tap<TValue>(
        this Task<Result<TValue>> resultTask,
        Action<TValue> action)
    {
        var result = await resultTask;
        return result.Tap(action);
    }

    /// <summary>
    /// Awaits the task, then converts the result to an <see cref="IActionResult"/>.
    /// Success → <c>200 OK</c>. Failure → <c>ProblemDetails</c>.
    /// </summary>
    public static async Task<IActionResult> ToActionResultAsync<T>(
        this Task<Result<T>> resultTask)
    {
        var result = await resultTask;
        return result.ToActionResult();
    }

    /// <summary>
    /// Awaits the task, then converts the result to an <see cref="IActionResult"/> with a custom success status code.
    /// Success → <paramref name="statusCode"/>. Failure → <c>ProblemDetails</c>.
    /// </summary>
    public static async Task<IActionResult> ToActionResultAsync<T>(
        this Task<Result<T>> resultTask, int statusCode)
    {
        var result = await resultTask;
        return result.ToActionResult(statusCode);
    }

    /// <summary>
    /// Awaits a <see cref="Result{T}"/> of <see cref="Unit"/> task, then converts the result to an <see cref="IActionResult"/>.
    /// Success → <c>204 No Content</c>. Failure → <c>ProblemDetails</c>.
    /// </summary>
    public static async Task<IActionResult> ToActionResultAsync(
        this Task<Result<Unit>> resultTask)
    {
        var result = await resultTask;
        return result.ToActionResult();
    }
}

# Changelog

All notable changes to this project are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [2.0.0] — 2026-04-01

### Breaking Changes

- **Target framework** upgraded from `net8.0` to `net10.0`. Consuming projects must target .NET 10.
- **`Response<T>` removed.** Use `Result<TValue, Error>.ToActionResult()` or `Match` for custom response shapes.
- **`BaseController` removed.** Remove inheritance; use the `ToActionResult()` extension method directly.
- **`GlobalErrorHandlerMiddleware` replaced** by `IExceptionHandler`-based `GlobalExceptionHandler`. Register with `AddExceptionHandler<T>()` and `app.UseExceptionHandler()`.
- **`DbTransactionMiddleware` exception-handler overload removed.** The `Func<IServiceProvider, HttpContext, Exception, Task>` constructor parameter no longer exists. Use `IExceptionHandler` for exception handling.
- **`PagedRequest.CurrentPage` (1-based) renamed to `PageIndex` (0-based).** Aligns with TanStack Table's server-side pagination contract. `Skip` calculation updated accordingly.
- **`StringExtensions` deleted.** Methods used `Convert.*` which has inconsistent null vs invalid-input behavior. Use `int.TryParse`, `decimal.TryParse` etc. directly.
- **`DoubleExtensions` deleted.** `ToInt(this double)` wraps a single `Convert.ToInt32` call with no added value. Use `(int)x` or `Convert.ToInt32(x)` directly.
- **`ObjectExtensions.ToJson()` removed.** Leaked `Newtonsoft.Json` types into caller APIs. Use `System.Text.Json.JsonSerializer.Serialize(obj)` directly.
- **`DateTimeExtensions.DateNow()` removed.** Hardcoded UTC+3 offset makes it unsuitable as a general-purpose utility. Use `TimeZoneInfo.ConvertTime()` explicitly. `IsWeekday()`, `IsWeekend()`, and `ToDateOnly()` are retained.
- **`QueryableExtensions.ToHashSetAsync()` removed.** EF Core 10 provides a native async `ToHashSetAsync()`.
- **`Newtonsoft.Json` package reference removed.** The library now uses `System.Text.Json` exclusively.

### Added

- **`Result<TValue, TError>`** discriminated union with `Match`, `MatchAsync`, `Handle`, `HandleAsync`, `Map`, `Tap`, `Bind`, and `GetValueOrDefault`.
- **`Result<TValue>`** shorthand type (equivalent to `Result<TValue, Error>`) for application code that always uses the library's error hierarchy.
- **Error hierarchy** — `abstract record Error` with concrete subtypes: `NotFoundError`, `AlreadyExistsError`, `ConflictError`, `UnauthorizedError`, `ForbiddenError`. Each carries contextual properties via positional constructor and owns its HTTP representation through virtual `StatusCode`, `Detail`, and `Code` properties. Subclass any type to add domain-specific context or override HTTP semantics without modifying library code.
- **`Unit`** struct for void operations (`Result<Unit, Error>`).
- **`ResultExtensions`** — `ToActionResult()`, `ToActionResult(int statusCode)`, `ToActionResultAsync()`, and the full async pipeline (`BindAsync`, `Map`, `Tap` Task overloads). Error-to-HTTP mapping reads `StatusCode`, `Detail`, and `Code` directly from the error instance — no switch expression, extensible without library changes.
- **`GlobalExceptionHandler`** — default `IExceptionHandler` implementation that logs unhandled exceptions and returns a 500 ProblemDetails response.
- **`SortColumn`** record — mirrors TanStack Table's `{ id, desc }` sort state shape.
- **`SortedPagedRequest`** — extends `PagedRequest` with `IReadOnlyList<SortColumn> Sorting`. Multi-column sort applied automatically via expression trees + reflection in `ToPagedAsync`.
- **`EnumerableExtensions`** — `WhereIf`, `TakeIf`, `SkipIf` for `IEnumerable<T>` (mirrors `QueryableExtensions`).
- **`ConfigurationExtensions.GetRequired`** — returns a config value or throws `InvalidOperationException` when the key is missing. Fills the gap left by `GetRequiredSection` (sections only).
- **`HashExtensions.ToMd5`** — hashes a string to a lowercase MD5 hex string using UTF-8. For non-security use (cache keys, checksums, ETags).
- **`ClaimsPrincipalExtensions`** — `GetId<T>`, `GetRequiredId<T>`, `GetEmail`. Uses `IParsable<T>` constraint so callers choose the ID type (`int`, `Guid`, `long`, etc.) without separate overloads.
- **`BrotliHelper`** — static helper with `Compress`/`Decompress` overloads for `byte[]` and `string` (UTF-8 + Base64 for string form).
- **`QueryableExtensions.WhereIf`, `TakeIf`, `SkipIf`** already existed; `ToPagedAsync(SortedPagedRequest)` overload is new.
- **`ApiCommons.Tests`** project — xUnit + FluentAssertions + coverlet test suite covering all phases.

### Changed

- `DbTransactionMiddleware` updated to primary constructor syntax (C# 13).
- `QueryableExtensions.ToPagedAsync` verified compatible with EF Core 10.
- `Microsoft.EntityFrameworkCore` updated from `8.0.22` to `10.0.4`.

### Migration Guide — v1.x → v2.x

| # | What changed | Action required |
|---|---|---|
| 1 | `net8.0` no longer targeted | Upgrade consuming projects to .NET 10 |
| 2 | `BaseController` removed | Remove inheritance; use `ToActionResult()` extension instead |
| 3 | `Response<T>` removed | Use `Result<TValue, Error>.ToActionResult()` or `Match` for custom shapes |
| 4 | `GlobalErrorHandlerMiddleware` replaced | Use `AddGlobalExceptionHandler<T>()` + `UseGlobalErrorHandler()`, or register `IExceptionHandler` manually |
| 5 | `DbTransactionMiddleware` error-handler overload removed | Use `IExceptionHandler` for global exception handling |
| 6 | `PagedRequest.CurrentPage` (1-based) → `PageIndex` (0-based) | Update all usages; aligns with TanStack Table server-side pagination |
| 7 | `ObjectExtensions.ToJson()` removed | Use `JsonSerializer.Serialize(obj)` directly |
| 8 | `DateTimeExtensions.DateNow()` removed | Use `TimeZoneInfo.ConvertTime()` explicitly |
| 9 | `StringExtensions` removed | Use `int.TryParse`, `decimal.TryParse` etc. directly |
| 10 | `DoubleExtensions.ToInt()` removed | Use `(int)x` or `Convert.ToInt32(x)` directly |
| 11 | `QueryableExtensions.ToHashSetAsync()` removed | Use EF Core 10's built-in `.ToHashSetAsync()` |
| 12 | `Newtonsoft.Json` dependency removed | Use `System.Text.Json` exclusively |

**Before / After:**

```csharp
// v1 — error handling
app.UseGlobalErrorHandler(async (sp, ctx, ex) =>
{
    ctx.Response.StatusCode = 500;
    await ctx.Response.WriteAsync(Response<object>.Fail(...).ToJson());
});

// v2
builder.Services.AddGlobalExceptionHandler<MyExceptionHandler>();
app.UseGlobalErrorHandler();
```

```csharp
// v1 — controller response
Response<string> response = result.Match(s => s, e => string.Empty);
return response.ToActionResult();

// v2
return await _service.GetAsync(id, ct).ToActionResultAsync();
```

```csharp
// v1 — pagination (1-based)
var req = new PagedRequest(currentPage: 1, pageSize: 20);

// v2 — pagination (0-based, aligns with TanStack Table)
var req = new PagedRequest(pageIndex: 0, pageSize: 20);
```

---

## [1.1.0] — prior

- Initial release targeting .NET 8.

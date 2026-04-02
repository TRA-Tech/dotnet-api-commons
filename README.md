# dotnet-api-commons

Common classes and extensions for ASP.NET Core APIs — Result types, error hierarchy, pagination, middleware, and utility extensions.

| Library version | .NET version |
|---|---|
| 2.x | .NET 10 |
| 1.x | .NET 8 |

---

## Feature Overview

| Area | What's included |
|---|---|
| **Result** | `Result<TValue, TError>` discriminated union, `Result<TValue>` shorthand, fluent async pipeline |
| **Error hierarchy** | `NotFoundError`, `AlreadyExistsError`, `ConflictError`, `UnauthorizedError`, `ForbiddenError` |
| **Pagination** | `PagedRequest`, `SortedPagedRequest`, `PagedResult<T>`, `ToPagedAsync` |
| **Middlewares** | `DbTransactionMiddleware`, `GlobalExceptionHandler` (IExceptionHandler) |
| **Extensions** | `ConfigurationExtensions`, `ClaimsPrincipalExtensions`, `QueryableExtensions`, `EnumerableExtensions`, `DateTimeExtensions`, `DateOnlyExtensions` |
| **Helpers** | `HashHelper`, `BrotliHelper` |

---

## Usage

### Result\<TValue\>

A discriminated union that represents either a success value or an `Error`. Services return `Result<T>` instead of throwing exceptions, and controllers convert them to HTTP responses.

`Result<TValue>` is a shorthand for `Result<TValue, Error>`. The fully generic `Result<TValue, TError>` is available when you need a custom error base type.

#### Returning results from services

Both success values and errors convert to `Result<T>` implicitly — no wrapping needed:

```csharp
public async Task<Result<ProductDetailDto>> GetByIdAsync(int id, CancellationToken ct)
{
    var product = await _db.Products.FindAsync([id], ct);

    if (product is null)
        return new NotFoundError("Product");   // implicit conversion to Result

    return new ProductDetailDto { ... };        // implicit conversion to Result
}
```

#### Converting to HTTP responses

`ToActionResultAsync()` maps the result to an appropriate `IActionResult`:
- On success: returns the value with `200 OK` (or a custom status code)
- On failure: returns a `ProblemDetails` response using the error's `StatusCode`, `Detail`, and `Code`

```csharp
[HttpGet("{id:int}")]
public Task<IActionResult> GetById(int id, CancellationToken ct) =>
    _service.GetByIdAsync(id, ct).ToActionResultAsync();

// Custom success status code (e.g. 201 Created)
[HttpPost]
public Task<IActionResult> Create([FromBody] CreateProductRequest req, CancellationToken ct) =>
    _service.CreateAsync(req, ct).ToActionResultAsync(201);
```

#### Void operations with Result\<Unit\>

Use `Result<Unit>` when the operation succeeds with no meaningful return value. `ToActionResultAsync()` returns `204 No Content` on success:

```csharp
public async Task<Result<Unit>> DiscontinueAsync(int id, CancellationToken ct)
{
    var product = await _db.Products.FindAsync([id], ct);

    if (product is null)
        return new NotFoundError("Product");

    if (product.Discontinued)
        return new ConflictError($"Product '{product.ProductName}' is already discontinued.");

    product.Discontinued = true;
    await _db.SaveChangesAsync(ct);
    return Unit.Value;
}

[HttpPatch("{id:int}/discontinue")]
public Task<IActionResult> Discontinue(int id, CancellationToken ct) =>
    _service.DiscontinueAsync(id, ct).ToActionResultAsync();
```

#### Transforming results — Map, Bind, Tap

| Method | Purpose |
|---|---|
| `Map(func)` | Transform the success value. Error passes through unchanged. |
| `BindAsync(func)` | Chain another `Result`-returning async operation. Short-circuits on error. |
| `Tap(action)` | Execute a side-effect on success (e.g. logging) without altering the result. |

```csharp
// Map — project a DTO to a summary shape without a second DB call
[HttpGet("{id:int}/summary")]
public Task<IActionResult> GetSummary(int id, CancellationToken ct) =>
    _service.GetByIdAsync(id, ct)
        .Map(dto => new { dto.ProductId, dto.ProductName, dto.UnitPrice })
        .ToActionResultAsync();

// BindAsync — chain two independent async lookups
[HttpGet("{id:int}/with-supplier")]
public Task<IActionResult> GetWithSupplier(int id, CancellationToken ct) =>
    _service.GetByIdAsync(id, ct)
        .BindAsync(product => _service.GetWithSupplierAsync(product.ProductId, ct))
        .ToActionResultAsync();

// Full pipeline — BindAsync + Map + Tap
[HttpGet("{id:int}/invoice-line")]
public Task<IActionResult> GetInvoiceLine(int id, CancellationToken ct) =>
    _service.GetByIdAsync(id, ct)
        .BindAsync(product => _service.GetWithSupplierAsync(product.ProductId, ct))
        .Map(ps => new InvoiceLineDto { ProductName = ps.ProductName, SupplierName = ps.SupplierName })
        .Tap(line => _logger.LogInformation("Invoice line generated for {Product}", line.ProductName))
        .ToActionResultAsync();
```

#### Pattern matching — Match and Handle

When you need full control over both branches, use `Match` (returns a value) or `Handle` (executes side-effects):

```csharp
// Match — custom response per error type
[HttpPatch("{id:int}/price")]
public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceRequest req, CancellationToken ct)
{
    var result = await _service.UpdatePriceAsync(id, req.NewPrice, ct);
    return result.Match<IActionResult>(
        _ => NoContent(),
        error => error switch
        {
            NotFoundError  => NotFound(new { error.Detail }),
            ConflictError  => Conflict(new { error.Detail }),
            _              => StatusCode(500, new { error.Detail })
        });
}

// GetValueOrDefault — return a fallback instead of propagating the error
var product = result.GetValueOrDefault(ProductDetailDto.Empty);
```

Both `Match` and `Handle` have async variants: `MatchAsync` and `HandleAsync`.

### Error hierarchy

`Error` is an abstract record with three virtual properties that control HTTP representation:

| Property | Default | Purpose |
|---|---|---|
| `StatusCode` | `500` | HTTP status code |
| `Detail` | `null` | Human-readable explanation (RFC 9457 `detail` field) |
| `Code` | `null` | Machine-readable code (placed in ProblemDetails `extensions`) |

#### Built-in error types

| Error type | HTTP Status | `detail` | `code` |
|---|---|---|---|
| `NotFoundError("Product")` | 404 | "Product was not found." | `PRODUCT_NOT_FOUND` |
| `AlreadyExistsError("Email")` | 409 | "Email already exists." | `EMAIL_ALREADY_EXISTS` |
| `ConflictError("Version mismatch")` | 409 | "Version mismatch" | `CONFLICT` |
| `UnauthorizedError("Token expired")` | 401 | "Token expired" | `UNAUTHORIZED` |
| `ForbiddenError("Order", "Delete")` | 403 | "Delete on Order is forbidden." | `ORDER_DELETE_FORBIDDEN` |

#### Custom error types

Subclass `Error` and override any property to define domain-specific errors:

```csharp
public sealed record PaymentFailedError(string Reason) : Error
{
    public override int StatusCode => 402;
    public override string? Detail => Reason;
    public override string? Code => "PAYMENT_FAILED";
}
```

An `Error` subclass with no overrides defaults to `500` with no `detail` or `code` in the response.

---

### Pagination

Server-side pagination primitives that align with [TanStack Table](https://tanstack.com/table)'s pagination and sorting contract.

#### PagedRequest

Uses 0-based page indexing. `PageSize` is clamped to `[1, MaxPageSize]` (default max: 200), and negative `PageIndex` values are normalized to 0.

```csharp
[HttpGet]
public async Task<IActionResult> List([FromQuery] PagedRequest req, CancellationToken ct)
{
    var result = await _db.Products.ToPagedAsync(req, ct);
    return Ok(result);
}
```

#### SortedPagedRequest

Extends `PagedRequest` with multi-column sort state. Each `SortColumn` has an `Id` (property name, case-insensitive) and a `Desc` flag. Sorting is applied automatically via expression trees — no extra dependencies needed. Unknown property names are silently ignored.

```csharp
// TanStack Table sends:
// { pageIndex: 0, pageSize: 20, sorting: [{ id: "categoryName", desc: false }] }
[HttpPost("list")]
public Task<IActionResult> List([FromBody] SortedPagedRequest req, CancellationToken ct) =>
    _service.GetCategoriesPagedAsync(req, ct).ToActionResultAsync();
```

Service implementation:

```csharp
public async Task<Result<PagedResult<CategoryListItemDto>>> GetCategoriesPagedAsync(
    SortedPagedRequest request, CancellationToken ct = default)
{
    var query = db.Categories
        .AsNoTracking()
        .Select(c => new CategoryListItemDto
        {
            CategoryName = c.CategoryName,
            Description  = c.Description
        });

    return await query.ToPagedAsync(request, ct);  // sorting + paging applied
}
```

#### ToPagedAsync

Extension method on `IQueryable<T>` that executes two queries: `CountAsync` for the total, then a paged `ToListAsync`. Returns `PagedResult<T>.Empty(request)` when the count is zero to avoid an unnecessary items query.

#### PagedResult\<T\>

The response shape returned to the client:

```json
{
  "items": [...],
  "totalCount": 42,
  "pageIndex": 0,
  "pageSize": 20,
  "totalPages": 3
}
```

`TotalPages` is computed as the integer ceiling of `TotalCount / PageSize`.

---

### Middlewares

#### GlobalExceptionHandler

A default `IExceptionHandler` implementation that logs unhandled exceptions (including request method and path) and returns a `500 ProblemDetails` response.

Use the built-in handler directly, or implement your own `IExceptionHandler` for custom error mapping. The library provides convenience extension methods for registration:

```csharp
// Program.cs — using convenience extensions
builder.Services.AddGlobalExceptionHandler<GlobalExceptionHandler>(); // registers ProblemDetails + handler
app.UseGlobalErrorHandler();
```

This is equivalent to the standard ASP.NET Core registration:

```csharp
// Program.cs — manual registration
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
app.UseExceptionHandler();
```

To customize error handling, implement `IExceptionHandler` and register your own handler instead:

```csharp
internal sealed class MyExceptionHandler(ILogger<MyExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Unhandled exception for {Method} {Path}",
            context.Request.Method, context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title  = "An unexpected error occurred."
            }, ct);

        return true;
    }
}

// Program.cs
builder.Services.AddGlobalExceptionHandler<MyExceptionHandler>();
app.UseGlobalErrorHandler();
```

#### DbTransactionMiddleware

Wraps controller actions in an EF Core database transaction. The middleware reads the `[DbTransaction]` attribute from the endpoint metadata, resolves the specified `DbContext` from DI, and begins a transaction. It commits on success and rolls back on any exception (letting the exception propagate to `IExceptionHandler`).

Actions without the attribute are passed through with no overhead.

Registration:

```csharp
// Program.cs
app.UseDbTransaction();
```

Usage:

```csharp
[HttpPost]
[DbTransaction(typeof(NorthwindDbContext))]
public async Task<IActionResult> Create([FromBody] CreateRequest req, CancellationToken ct)
{
    // everything in this action runs inside a single database transaction
}
```

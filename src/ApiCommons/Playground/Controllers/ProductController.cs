using ApiCommons.Result;
using Microsoft.AspNetCore.Mvc;
using Playground.Dtos.Product;
using Playground.Services;

namespace Playground.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService productService, ILogger<ProductController> logger) : ControllerBase
{
    // Demonstrates: basic Result<T> → ToActionResult (200 OK or 404 ProblemDetails)
    [HttpGet("{id:int}")]
    public Task<IActionResult> GetById(int id, CancellationToken ct = default) =>
        productService.GetByIdAsync(id, ct).ToActionResultAsync();

    // Demonstrates: Map — transforms the success value into a different shape
    // without touching the error path.
    [HttpGet("{id:int}/price-tag")]
    public Task<IActionResult> GetPriceTag(int id, CancellationToken ct = default) =>
        productService.GetByIdAsync(id, ct)
            .Map(p => new
            {
                p.ProductName,
                PriceTag = p.UnitPrice.HasValue ? $"${p.UnitPrice:F2}" : "Price not set",
                InStock = p.UnitsInStock > 0
            })
            .ToActionResultAsync();

    // Demonstrates: BindAsync — chains two independent Result-returning async
    // operations. If GetByIdAsync fails (404), GetWithSupplierAsync is never called.
    [HttpGet("{id:int}/with-supplier")]
    public Task<IActionResult> GetWithSupplier(int id, CancellationToken ct = default) =>
        productService.GetByIdAsync(id, ct)
            .BindAsync(product => productService.GetWithSupplierAsync(product.ProductId, ct))
            .ToActionResultAsync();

    // Demonstrates: Tap — performs a side-effect (logging) on the success value
    // without altering the result flowing through the pipeline.
    [HttpPut("{id:int}/price")]
    public Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceRequest request, CancellationToken ct = default) =>
        productService.UpdatePriceAsync(id, request.NewPrice, ct)
            .Tap(p => logger.LogInformation(
                "[Controller] Price updated for '{Name}' — new price: {Price}",
                p.ProductName, p.UnitPrice))
            .ToActionResultAsync();

    // Demonstrates: Match — explicitly branches on success/failure to produce
    // a custom IActionResult. Also shows ConflictError (409) in action.
    [HttpPost("{id:int}/discontinue")]
    public async Task<IActionResult> Discontinue(int id, CancellationToken ct = default)
    {
        var result = await productService.DiscontinueAsync(id, ct);

        return result.Match<IActionResult>(
            _ => NoContent(),
            error => error switch
            {
                NotFoundError => NotFound(new { error.Detail }),
                ConflictError => Conflict(new { error.Detail }),
                _ => StatusCode(500, new { error.Detail })
            });
    }

    // Demonstrates: GetValueOrDefault — returns a fallback DTO when the
    // product is not found, instead of propagating the error.
    [HttpGet("{id:int}/price-or-default")]
    public async Task<IActionResult> GetPriceOrDefault(int id, CancellationToken ct = default)
    {
        var fallback = new ProductDetailDto
        {
            ProductId = id,
            ProductName = "Unknown",
            UnitPrice = 0m,
            Discontinued = false
        };

        var result = await productService.GetByIdAsync(id, ct);
        var dto = result.GetValueOrDefault(fallback);
        return Ok(dto);
    }

    // Demonstrates: full async pipeline — BindAsync + Map + Tap chained together.
    // 1. GetByIdAsync       → fetch product (or 404)
    // 2. BindAsync          → fetch supplier info (or 404)
    // 3. Map                → project into InvoiceLineDto
    // 4. Tap                → log the generated invoice line
    // 5. ToActionResultAsync → convert to 200 OK or ProblemDetails
    [HttpGet("{id:int}/invoice-line")]
    public Task<IActionResult> GetInvoiceLine(int id, CancellationToken ct = default) =>
        productService.GetByIdAsync(id, ct)
            .BindAsync(product => productService.GetWithSupplierAsync(product.ProductId, ct))
            .Map(ps => new InvoiceLineDto
            {
                ProductId = ps.ProductId,
                ProductName = ps.ProductName,
                UnitPrice = ps.UnitPrice ?? 0m,
                SupplierName = ps.SupplierName,
                PriceTag = ps.UnitPrice.HasValue ? $"${ps.UnitPrice:F2}" : "N/A"
            })
            .Tap(line => logger.LogInformation(
                "[InvoiceLine] {Product} from {Supplier} @ {Price}",
                line.ProductName, line.SupplierName, line.PriceTag))
            .ToActionResultAsync();
}

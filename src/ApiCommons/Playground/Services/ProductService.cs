using ApiCommons.Result;
using Microsoft.EntityFrameworkCore;
using Playground.Dtos.Product;
using Playground.Entities;

namespace Playground.Services;

public interface IProductService
{
    Task<Result<ProductDetailDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<ProductWithSupplierDto>> GetWithSupplierAsync(int id, CancellationToken ct = default);
    Task<Result<ProductDetailDto>> UpdatePriceAsync(int id, decimal newPrice, CancellationToken ct = default);
    Task<Result<Unit>> DiscontinueAsync(int id, CancellationToken ct = default);
}

public class ProductService(NorthwindDbContext db, ILogger<ProductService> logger) : IProductService
{
    public async Task<Result<ProductDetailDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id, ct);

        if (product is null)
            return new NotFoundError("Product");

        return ToDetailDto(product);
    }

    public async Task<Result<ProductWithSupplierDto>> GetWithSupplierAsync(int id, CancellationToken ct = default)
    {
        var product = await db.Products
            .AsNoTracking()
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == id, ct);

        if (product is null)
            return new NotFoundError("Product");

        if (product.Supplier is null)
            return new NotFoundError("Supplier");

        return new ProductWithSupplierDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            UnitPrice = product.UnitPrice,
            SupplierName = product.Supplier.CompanyName,
            SupplierPhone = product.Supplier.Phone,
            SupplierCountry = product.Supplier.Country
        };
    }

    public async Task<Result<ProductDetailDto>> UpdatePriceAsync(int id, decimal newPrice, CancellationToken ct = default)
    {
        var product = await db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id, ct);

        if (product is null)
            return new NotFoundError("Product");

        var oldPrice = product.UnitPrice;
        product.UnitPrice = newPrice;
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Product {Id} price changed: {Old} -> {New}", id, oldPrice, newPrice);

        return ToDetailDto(product);
    }

    public async Task<Result<Unit>> DiscontinueAsync(int id, CancellationToken ct = default)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id, ct);

        if (product is null)
            return new NotFoundError("Product");

        if (product.Discontinued)
            return new ConflictError($"Product '{product.ProductName}' is already discontinued.");

        product.Discontinued = true;
        await db.SaveChangesAsync(ct);

        return Unit.Value;
    }

    private static ProductDetailDto ToDetailDto(Product product) => new()
    {
        ProductId = product.ProductId,
        ProductName = product.ProductName,
        UnitPrice = product.UnitPrice,
        UnitsInStock = product.UnitsInStock,
        Discontinued = product.Discontinued,
        CategoryName = product.Category?.CategoryName
    };
}

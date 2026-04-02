namespace Playground.Dtos.Product;

public sealed class ProductWithSupplierDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = "";
    public decimal? UnitPrice { get; init; }
    public string SupplierName { get; init; } = "";
    public string? SupplierPhone { get; init; }
    public string? SupplierCountry { get; init; }
}

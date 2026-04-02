namespace Playground.Dtos.Product;

public sealed class ProductDetailDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = "";
    public decimal? UnitPrice { get; init; }
    public short? UnitsInStock { get; init; }
    public bool Discontinued { get; init; }
    public string? CategoryName { get; init; }
}

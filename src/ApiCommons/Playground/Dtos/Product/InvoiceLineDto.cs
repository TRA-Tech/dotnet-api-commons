namespace Playground.Dtos.Product;

public sealed class InvoiceLineDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = "";
    public decimal UnitPrice { get; init; }
    public string SupplierName { get; init; } = "";
    public string PriceTag { get; init; } = "";
}

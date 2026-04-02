namespace Playground.Dtos.Category;

public sealed class CategoryDetailDto
{
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = "";
    public string? Description { get; init; }
    public bool HasPicture { get; init; }
    public int ProductCount { get; init; }
}

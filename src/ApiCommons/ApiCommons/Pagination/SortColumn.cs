namespace ApiCommons.Pagination;

/// <summary>
/// Represents a single sort column, mirroring TanStack Table's <c>{ id, desc }</c> sort state shape.
/// </summary>
public sealed class SortColumn
{
    /// <summary>The property name to sort by (case-insensitive).</summary>
    public string Id { get; set; } = "";

    /// <summary>True for descending order; false for ascending.</summary>
    public bool Desc { get; set; }
}

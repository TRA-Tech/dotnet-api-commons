namespace ApiCommons.Pagination;

/// <summary>
/// Represents a server-side pagination request using 0-based page indexing,
/// aligning with TanStack Table's server-side pagination contract.
/// </summary>
public class PagedRequest
{
    /// <summary>
    /// The 0-based index of the current page. Values less than 0 are normalized to 0.
    /// </summary>
    public int PageIndex { get; init; } = 0;

    /// <summary>
    /// The number of items to return per page. Clamped to the range [1, <see cref="MaxPageSize"/>].
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// The upper bound for <see cref="PageSize"/>. Values less than 1 are normalized to 1.
    /// </summary>
    public int MaxPageSize { get; init; } = 200;

    /// <summary>
    /// The number of items to skip, computed as <c>PageIndex * PageSize</c>.
    /// Pass directly to LINQ <c>Skip</c> before <c>Take</c>.
    /// </summary>
    public int Skip => PageIndex * PageSize;

    public PagedRequest() { }

    /// <summary>
    /// Initializes a new instance with explicit values. All inputs are normalized.
    /// </summary>
    /// <param name="pageIndex">0-based page index; values less than 0 become 0.</param>
    /// <param name="pageSize">Page size; clamped to [1, <paramref name="maxPageSize"/>].</param>
    /// <param name="maxPageSize">Upper bound for <see cref="PageSize"/>; values less than 1 become 1.</param>
    public PagedRequest(int pageIndex, int pageSize, int maxPageSize = 200)
    {
        MaxPageSize = Math.Max(maxPageSize, 1);
        PageIndex   = Math.Max(pageIndex, 0);
        PageSize    = Math.Clamp(pageSize, 1, MaxPageSize);
    }
}

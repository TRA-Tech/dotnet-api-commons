namespace ApiCommons.Pagination;

/// <summary>
/// Represents a paged result set returned from a query, including the materialized items,
/// total item count, paging metadata, and a computed total page count.
/// </summary>
/// <typeparam name="T">The element type of the paged items.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>The materialized items for the requested page.</summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>The total number of items across all pages.</summary>
    public int TotalCount { get; init; }

    /// <summary>The 0-based page index used to produce this result.</summary>
    public int PageIndex { get; init; }

    /// <summary>The page size used to produce this result.</summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The total number of pages, computed as the integer ceiling of
    /// <c>TotalCount / PageSize</c>. Returns 0 when <see cref="PageSize"/> is not positive.
    /// </summary>
    public int TotalPages => PageSize <= 0 ? 0 : (TotalCount + PageSize - 1) / PageSize;

    private PagedResult() { }

    /// <summary>
    /// Initializes a new instance from a list of items and a <see cref="PagedRequest"/>.
    /// </summary>
    /// <param name="items">The materialized items for the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="request">The pagination request that produced this page.</param>
    public PagedResult(IReadOnlyList<T> items, int totalCount, PagedRequest request)
    {
        Items      = items ?? [];
        TotalCount = totalCount;
        PageIndex  = request.PageIndex;
        PageSize   = request.PageSize;
    }

    /// <summary>
    /// Creates a new <see cref="PagedResult{T}"/> from a list of items and a <see cref="PagedRequest"/>.
    /// </summary>
    public static PagedResult<T> From(IReadOnlyList<T> items, int totalCount, PagedRequest request) =>
        new(items, totalCount, request);

    /// <summary>
    /// Creates an empty <see cref="PagedResult{T}"/> for the given <see cref="PagedRequest"/>.
    /// Use when the total count is zero to avoid an unnecessary items query.
    /// </summary>
    public static PagedResult<T> Empty(PagedRequest request) =>
        new()
        {
            Items      = [],
            TotalCount = 0,
            PageIndex  = request.PageIndex,
            PageSize   = request.PageSize
        };
}

namespace ApiCommons.Pagination
{
    /// <summary>
    /// Represents a paged result set returned from a query, including the materialized items,
    /// total item count, paging metadata (current page and page size), and computed total pages.
    /// </summary>
    /// <typeparam name="T">The element type contained in the paged items.</typeparam>
    public sealed class PagedResult<T>
    {
        /// <summary>
        /// The items for the requested page. Materialized as a read-only list.
        /// </summary>
        public IReadOnlyList<T> Items { get; init; } = [];

        /// <summary>
        /// The total number of items in the full result set (across all pages).
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// The 1-based index of the current page used to produce this result.
        /// </summary>
        public int CurrentPage { get; init; }

        /// <summary>
        /// The page size used to produce this result.
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// The total number of pages, computed as an integer ceiling of <c>TotalCount / PageSize</c>.
        /// Returns 0 when <see cref="PageSize"/> is not positive.
        /// </summary>
        public int TotalPages => PageSize <= 0 ? 0 : (TotalCount + PageSize - 1) / PageSize;

        /// <summary>
        /// Private parameterless constructor used by factory methods.
        /// </summary>
        private PagedResult() { }

        /// <summary>
        /// Initializes a new instance of <see cref="PagedResult{T}"/> from items and a <see cref="PagedRequest"/>,

        /// binding <see cref="CurrentPage"/> and <see cref="PageSize"/> from the request.
        /// </summary>
        /// <param name="items">Materialized items for the current page.</param>
        /// <param name="totalCount">Total number of items across all pages.</param>
        /// <param name="request">The pagination request that provided the page and page size.</param>
        public PagedResult(IReadOnlyList<T> items, int totalCount, PagedRequest request)
        {
            Items = items ?? [];
            TotalCount = totalCount;
            CurrentPage = request.CurrentPage;
            PageSize = request.PageSize;
        }

        /// <summary>
        /// Creates a new <see cref="PagedResult{T}"/> from items and a <see cref="PagedRequest"/>.
        /// </summary>
        /// <param name="items">Materialized items for the current page.</param>
        /// <param name="totalCount">Total number of items across all pages.</param>
        /// <param name="request">The pagination request that provided the page and page size.</param>
        /// <returns>A new <see cref="PagedResult{T}"/> instance.</returns>
        public static PagedResult<T> From(IReadOnlyList<T> items, int totalCount, PagedRequest request) =>
            new(items, totalCount, request);

        /// <summary>
        /// Creates an empty <see cref="PagedResult{T}"/> for the given <see cref="PagedRequest"/>.
        /// Useful when the total count is zero.
        /// </summary>
        /// <param name="request">The pagination request that provided the page and page size.</param>
        /// <returns>An empty <see cref="PagedResult{T}"/> with zero items.</returns>
        public static PagedResult<T> Empty(PagedRequest request) =>
            new()
            {
                Items = [],
                TotalCount = 0,
                CurrentPage = request.CurrentPage,
                PageSize = request.PageSize
            };
    }
}

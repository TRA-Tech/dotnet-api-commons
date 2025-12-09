namespace ApiCommons.Pagination
{
    /// <summary>
    /// Represents a request model for server-side pagination, defining the current page,
    /// page size, and upper bound for the page size. Provides a computed <see cref="Skip"/>
    /// value to apply <c>Skip/Take</c> in LINQ queries.
    /// </summary>
    public sealed class PagedRequest
    {
        /// <summary>
        /// The 1-based index of the current page. Values less than 1 are normalized to 1.
        /// </summary>
        public int CurrentPage { get; init; } = 1;

        /// <summary>
        /// The number of items to return per page. It is clamped to the range [1, <see cref="MaxPageSize"/>].
        /// </summary>
        public int PageSize { get; init; } = 20;

        /// <summary>
        /// The maximum allowed page size used to clamp <see cref="PageSize"/>.
        /// </summary>
        public int MaxPageSize { get; init; } = 200;

        /// <summary>
        /// The number of items to skip, computed as <c>(CurrentPage - 1) * PageSize</c>.
        /// Use this value with LINQ <c>Skip</c> before <c>Take</c>.
        /// </summary>
        public int Skip => (CurrentPage - 1) * PageSize;
        
        /// <summary>
        /// Initializes a new instance of <see cref="PagedRequest"/> with default values.
        /// </summary>
        public PagedRequest() { }

        /// <summary>
        /// Initializes a new instance of <see cref="PagedRequest"/> with explicit values.
        /// Values are normalized using <see cref="Math.Max(int, int)"/> and <see cref="Math.Clamp(int, int, int)"/>.
        /// </summary>
        /// <param name="page">Requested 1-based page index; values less than 1 become 1.</param>
        /// <param name="pageSize">Requested page size; clamped to the range [1, <paramref name="maxPageSize"/>].</param>
        /// <param name="maxPageSize">Upper bound for <see cref="PageSize"/>; values less than 1 become 1.</param>
        public PagedRequest(int page, int pageSize, int maxPageSize = 200)
        {
            CurrentPage = Math.Max(page, 1);
            MaxPageSize = Math.Max(maxPageSize, 1);
            PageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        }
    }
}

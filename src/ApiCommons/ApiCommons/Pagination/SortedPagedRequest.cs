namespace ApiCommons.Pagination;

/// <summary>
/// Extends <see cref="PagedRequest"/> with multi-column sort state,
/// mirroring TanStack Table's server-side pagination contract.
/// </summary>
public sealed class SortedPagedRequest : PagedRequest
{
    /// <summary>
    /// The sort columns to apply. An empty list means no ordering is applied.
    /// </summary>
    public IReadOnlyList<SortColumn> Sorting { get; init; } = [];

    public SortedPagedRequest() { }

    public SortedPagedRequest(
        int pageIndex,
        int pageSize,
        IReadOnlyList<SortColumn>? sorting = null,
        int maxPageSize = 200)
        : base(pageIndex, pageSize, maxPageSize)
    {
        Sorting = sorting ?? [];
    }
}


namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// PagedResult
    /// </summary>
    public class PagedResult
    {
        /// <summary>
        /// Gets or sets the queryable.
        /// </summary>
        /// <value>
        /// The queryable.
        /// </value>
        public IQueryable Queryable { get; set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the page count.
        /// </summary>
        /// <value>
        /// The page count.
        /// </value>
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>
        /// The row count.
        /// </value>
        public int RowCount { get; set; }
    }

    /// <summary>
    /// PagedResult{TSource}
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public class PagedResult<TSource> : PagedResult
    {
        /// <summary>
        /// Gets or sets the queryable.
        /// </summary>
        /// <value>
        /// The queryable.
        /// </value>
        public new IQueryable<TSource> Queryable { get; set; }
    }
}
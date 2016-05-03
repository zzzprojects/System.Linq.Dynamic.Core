
namespace System.Linq.Dynamic.Core
{
    public class PagedResult
    {
        public IQueryable Queryable { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int RowCount { get; set; }
    }

    public class PagedResult<TSource>
    {
        public IQueryable<TSource> Queryable { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int RowCount { get; set; }
    }
}
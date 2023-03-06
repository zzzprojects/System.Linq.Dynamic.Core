using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Helpers.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string? X { get; set; }
        public string Name { get; set; }
        public int? NullableInt { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

#if NET4 || NET452
        public DateTime Created { get; set; }
#else
        public DateTimeOffset Created { get; set; }
#endif
    }
}
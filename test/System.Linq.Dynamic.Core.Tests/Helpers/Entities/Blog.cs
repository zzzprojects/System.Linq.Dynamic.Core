using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Helpers.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public int? NullableInt { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}

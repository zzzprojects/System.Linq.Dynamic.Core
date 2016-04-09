
namespace System.Linq.Dynamic.Core.Tests.Helpers.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        public DateTime PostDate { get; set; }

        public int NumberOfReads { get; set; }
    }
}
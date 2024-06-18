using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace System.Linq.Dynamic.Core.Tests.Helpers.Entities;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int PostId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public int BlogId { get; set; }

    public virtual Blog Blog { get; set; }

    public int NumberOfReads { get; set; }

    public DateTime PostDate { get; set; }
}
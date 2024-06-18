using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace System.Linq.Dynamic.Core.Tests.Helpers.Entities;

public class Blog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int BlogId { get; set; }

    public string? X { get; set; }

    public string Name { get; set; }

    public int? NullableInt { get; set; }

    public virtual ICollection<Post> Posts { get; set; }

#if NET461 || NET48
    public DateTime Created { get; set; }
#else
    public DateTimeOffset Created { get; set; }
#endif
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleAppEF2.Database
{
    public class ProductDynamic
    {
        [Key]
        public int Key { get; set; }

        [NotMapped]
        public dynamic Properties { get; set; }

        public int? NullableInt { get; set; }

        public Dictionary<string, object> Dict { get; set; }
    }
}

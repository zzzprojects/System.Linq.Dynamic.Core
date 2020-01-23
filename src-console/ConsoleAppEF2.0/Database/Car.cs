using System;
using System.ComponentModel.DataAnnotations;

namespace ConsoleAppEF2.Database
{
    public class Car
    {
        [Key]
        public int Key { get; set; }

        [Required]
        [StringLength(8)]
        public string Vin { get; set; }

        [Required]
        public string Year { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Color { get; set; }

        public string Extra { get; set; }

        [Required]
        public DateTime DateLastModified { get; set; }

        public DateTime? DateDeleted { get; set; }

        public int? NullableInt { get; set; }

        public string X(bool b, string s)
        {
            return b + s + Color;
        }
    }
}

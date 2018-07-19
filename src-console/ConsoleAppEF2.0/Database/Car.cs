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

        [Required]
        public DateTime DateLastModified { get; set; }
    }
}

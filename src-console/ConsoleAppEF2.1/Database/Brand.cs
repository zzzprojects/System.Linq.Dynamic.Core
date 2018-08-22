using System.ComponentModel.DataAnnotations;

namespace ConsoleAppEF21.Database
{
    public class Brand
    {
        [Key]
        public string BrandType { get; set; }

        [Required]
        public string BrandName { get; set; }
    }
}

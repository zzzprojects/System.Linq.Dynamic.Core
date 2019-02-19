using System.ComponentModel.DataAnnotations;

namespace ConsoleAppEF2.Database
{
    public class BaseDto
    {
        [Key]
        public int Key { get; set; }

        public string BaseName { get; set; }
    }
}

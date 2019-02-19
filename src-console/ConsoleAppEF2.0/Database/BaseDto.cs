using System.ComponentModel.DataAnnotations;

namespace ConsoleAppEF2.Database
{
    public abstract class BaseDto
    {
        [Key]
        public int Key { get; set; }

        public string BaseName { get; set; }
    }
}

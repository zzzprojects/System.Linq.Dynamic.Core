using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsoleAppEF2.Database
{
    public class ComplexDto
    {
        [Key]
        public int Key { get; set; }

        public string X { get; set; }

        public IEnumerable<BaseDto> ListOfBaseDtos { get; set; }
    }
}

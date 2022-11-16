
namespace System.Linq.Dynamic.Core.Tests.Helpers.Models
{
    public class Person
    {
        public int Id { get; set; }

        public int? NullableId { get; set; }

        public string Name { get; set; }

        public DateTime D { get; set; }

        public DateTimeOffset O { get; set; }
    }
}
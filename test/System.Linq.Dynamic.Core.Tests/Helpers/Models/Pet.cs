
namespace System.Linq.Dynamic.Core.Tests.Helpers.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Person Owner { get; set; }
        public int OwnerId { get; set; }
        public int? NullableOwnerId { get; set; }
    }
}

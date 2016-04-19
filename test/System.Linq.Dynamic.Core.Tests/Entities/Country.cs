using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Entities
{
    public class Country : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
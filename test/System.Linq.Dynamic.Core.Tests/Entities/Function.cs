using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Entities
{
    public class Function : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public ICollection<SubFunction> SubFunctions { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
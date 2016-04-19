using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Entities
{
    public class Company : Entity
    {
        public string Name { get; set; }

        public long? MainCompanyId { get; set; }

        public MainCompany MainCompany { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
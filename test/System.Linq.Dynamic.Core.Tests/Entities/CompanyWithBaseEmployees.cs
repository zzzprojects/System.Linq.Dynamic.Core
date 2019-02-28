using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Entities
{
    public class CompanyWithBaseEmployees
    {
        public ICollection<BaseEmployee> Employees { get; set; }
    }
}

using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.ConsoleTestApp.net452.Entities
{
    public partial class Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Company()
        {
            KendoGridEmployee = new HashSet<Employee>();
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public long? MainCompanyId { get; set; }

        public virtual MainCompany MainCompany { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> KendoGridEmployee { get; set; }
    }
}

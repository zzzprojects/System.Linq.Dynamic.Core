using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.ConsoleTestApp.net452.Entities
{
    public partial class Country
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Country()
        {
            KendoGridEmployee = new HashSet<Employee>();
        }

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> KendoGridEmployee { get; set; }
    }
}

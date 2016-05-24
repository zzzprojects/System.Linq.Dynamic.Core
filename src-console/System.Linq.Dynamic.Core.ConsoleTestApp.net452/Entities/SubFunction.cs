using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.ConsoleTestApp.net452.Entities
{
    public partial class SubFunction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SubFunction()
        {
            Employee = new HashSet<Employee>();
        }

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long? FunctionId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employee { get; set; }

        public virtual Function Function { get; set; }
    }
}

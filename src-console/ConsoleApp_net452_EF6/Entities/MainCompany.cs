using System.Collections.Generic;

namespace ConsoleApp_net452_EF6.Entities
{
    public partial class MainCompany
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MainCompany()
        {
            KendoGridCompany = new HashSet<Company>();
        }

        public long Id { get; set; }

        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Company> KendoGridCompany { get; set; }
    }
}

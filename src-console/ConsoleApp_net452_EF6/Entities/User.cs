using System.Collections.Generic;

namespace ConsoleApp_net452_EF6.Entities
{
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Roles = new HashSet<Role>();
        }

        public long Id { get; set; }

        public string IdentityName { get; set; }

        public string DisplayName { get; set; }

        public string EmailAddress { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Role> Roles { get; set; }
    }
}

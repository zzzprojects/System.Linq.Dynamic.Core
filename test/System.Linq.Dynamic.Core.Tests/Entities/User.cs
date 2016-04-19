using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Entities
{
    public class User : Entity
    {
        public string IdentityName { get; set; }

        public string DisplayName { get; set; }

        public string EmailAddress { get; set; }

        public ICollection<Role> Roles { get; set; }

        public bool HasRole(string role)
        {
            return Roles.Any(r => r.Name == role);
        }
    }
}
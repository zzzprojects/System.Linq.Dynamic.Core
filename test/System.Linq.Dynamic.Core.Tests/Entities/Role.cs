using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Entities
{
    public class Role : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Helpers.Models
{
    public class Role
    {
        public static readonly Role[] StandardRoles = {
            new Role { Name="Admin", Permissions = new List<Permission> { new Permission { Name = "Admin" } } },
            new Role { Name="User", Permissions = new List<Permission> { new Permission { Name = "User" } } },
            new Role { Name="Guest", Permissions = new List<Permission> { new Permission { Name = "Guest" } } },
            new Role { Name="G", Permissions = new List<Permission> { new Permission { Name = "G" } } },
            new Role { Name="J", Permissions = new List<Permission> { new Permission { Name = "J" } } },
            new Role { Name="A", Permissions = new List<Permission> { new Permission { Name = "A" } } }
        };

        public Role()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Permission> Permissions { get; set; }
    }
}
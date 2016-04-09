using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Helpers
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public int Income { get; set; }

        public UserProfile Profile { get; set; }

        public List<Role> Roles { get; set; }

        public static IList<User> GenerateSampleModels(int total, bool allowNullableProfiles = false)
        {
            var list = new List<User>();

            for (int i = 0; i < total; i++)
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "User" + i,
                    Income = ((i) % 15) * 100
                };

                if (!allowNullableProfiles || (i % 8) != 5)
                {
                    user.Profile = new UserProfile
                    {
                        FirstName = "FirstName" + i,
                        LastName = "LastName" + i,
                        Age = (i % 50) + 18
                    };
                }

                user.Roles = new List<Role>(Role.StandardRoles);

                list.Add(user);
            }

            return list.ToArray();
        }
    }

    public class UserProfile
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Age { get; set; }
    }

    public class Role
    {
        public static readonly Role[] StandardRoles = {
            new Role { Name="Admin"},
            new Role { Name="User"},
            new Role { Name="Guest"},
            new Role { Name="G"},
            new Role { Name="J"},
            new Role { Name="A"},
        };

        public Role()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class SimpleValuesModel
    {
        public float FloatValue { get; set; }

        public decimal DecimalValue { get; set; }
    }
}
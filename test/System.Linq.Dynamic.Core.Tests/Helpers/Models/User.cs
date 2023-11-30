using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests.Helpers.Models;

public class User
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? LegalPersonId { get; set; }

    public Guid? PointSiteTD { get; set; }

    public SnowflakeId SnowflakeId { get; set; }

    public string UserName { get; set; }

    public DateTime BirthDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? NullableInt { get; set; }

    public int Income { get; set; }

    public char C { get; set; }

    public UserProfile Profile { get; set; }

    public UserState State { get; set; }

    public List<Role> Roles { get; set; }

    public bool TestMethod1()
    {
        return true;
    }

    public bool TestMethod2(User other)
    {
        return true;
    }

    public bool TestMethod3(User other)
    {
        return Id == other.Id;
    }

    public bool TryParseWithoutArgument(out string xxx)
    {
        return TryParseWithArgument(UserName, out xxx);
    }

    public bool TryParseWithArgument(string s, out string xxx)
    {
        if (s.EndsWith("1") || s.EndsWith("2"))
        {
            xxx = UserName;
            return true;
        }

        xxx = "";
        return false;
    }

    public bool TryParseWithArgumentAndTwoOut(string s, out string xxx, out int x)
    {
        x = 0;
        return TryParseWithArgument(s, out xxx) && int.TryParse(s, out x);
    }

    public static IList<User> GenerateSampleModels(int total, bool allowNullableProfiles = false)
    {
        var list = new List<User>();

        for (int i = 0; i < total; i++)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                SnowflakeId = new SnowflakeId(((ulong)long.MaxValue + (ulong)i + 2UL)),
                UserName = "User" + i,
                Income = 1 + (i % 15) * 100,
                BirthDate = DateTime.UtcNow.AddYears(-50),
                EndDate = i % 2 == 0 ? DateTime.UtcNow.AddYears(99) : null
            };

            if (!allowNullableProfiles || i % 8 != 5)
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
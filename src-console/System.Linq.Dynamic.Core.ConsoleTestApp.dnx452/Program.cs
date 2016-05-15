using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Dynamic.Core.Tests.Helpers;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.Entity;

namespace System.Linq.Dynamic.Core.ConsoleTestApp.dnx452
{
    public class Program
    {
        static readonly Random Rnd = new Random(1);
        private static BlogContext context;

        public static void Main(string[] args)
        {
            Console.WriteLine("--start");
            TestDB();

            Console.WriteLine("--end");
        }

        private static void TestDB()
        {
            try
            {
                var builder = new DbContextOptionsBuilder();
                //builder.UseSqlite($"Filename=SqlFunctionsTests_{Guid.NewGuid()}.db");
                builder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=SSPI;AttachDBFilename=C:\\Projects\\GitHub\\System.Linq.Dynamic.Core\\src-console\\System.Linq.Dynamic.Core.ConsoleTestApp.dnx452\\XSqlFunctionsTests.mdf");

                context = new BlogContext(builder.Options);
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                PopulateTestData(1, 0);

                const string search = "1";
                var expected = context.Blogs.AsQueryable().Where(b => System.Data.Entity.SqlServer.SqlFunctions.StringConvert((double)b.BlogId).Contains(search)).ToArray();

                var result = context.Blogs.Where("System.Data.Entity.SqlServer.SqlFunctions.StringConvert((double) BlogId).Contains(@0)", search).ToDynamicArray<Blog>();

                WriteArray(expected, result);
            }
            finally
            {
                context.Database.EnsureDeleted();
                context.Dispose();
                context = null;
            }
        }

        private static void PopulateTestData(int blogCount = 25, int postCount = 10)
        {
            for (int i = 0; i < blogCount; i++)
            {
                var blog = new Blog { Name = "Blog" + (i + 1), Created = DateTime.Today.AddDays(-Rnd.Next(0, 100)) };

                context.Blogs.Add(blog);

                for (int j = 0; j < postCount; j++)
                {
                    var post = new Post
                    {
                        Blog = blog,
                        Title = $"Blog {i + 1} - Post {j + 1}",
                        Content = "My Content",
                        PostDate = DateTime.Today.AddDays(-Rnd.Next(0, 100)).AddSeconds(Rnd.Next(0, 30000)),
                        NumberOfReads = Rnd.Next(0, 5000)
                    };

                    context.Posts.Add(post);
                }
            }

            context.SaveChanges();
        }

        private static void Write<T>(T check, T result) where T : class
        {
            Console.WriteLine("> '{0}'", check == result);
        }

        private static void Write(int check, int result)
        {
            Console.WriteLine("> {0} == {1} = '{2}'", check, result, check == result);
        }

        private static void WriteArray<T>(T[] check, T[] result) where T : class
        {
            for (int i = 0; i < check.Length; i++)
            {
                Console.WriteLine("> {0} : c={1}, r={2}  '{3}'", i, check[i], result[i], check[i] == result[i]);
            }
        }

        private static void WriteArray(Guid[] check, Guid[] result)
        {
            for (int i = 0; i < check.Length; i++)
            {
                Console.WriteLine("> {0} : c={1}, r={2}  '{3}'", i, check[i], result[i], check[i] == result[i]);
            }
        }

        private static void WriteArray(int[] check, int[] result)
        {
            for (int i = 0; i < check.Length; i++)
            {
                Console.WriteLine("> {0} : c={1}, r={2}  '{3}'", i, check[i], result[i], check[i] == result[i]);
            }
        }
    }
}
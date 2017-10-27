using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace QueryBug
{
    class Program
    {
        static string GetDebugView(Expression exp)
        {
            if (exp == null)
                return null;

            var propertyInfo = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic);
            return propertyInfo.GetValue(exp) as string;
        }

        static void Main(string[] args)
        {
            var context = new Context();
            context.Database.Migrate();
            context.RootEntities.Add(new RootEntity
            {
                Name = "Entity",
                ParentEntity = new ParentEntity
                {
                    Name = "Parent"
                },
                Children = new List<ChildEntity>
                {
                    new ChildEntity(),
                    new ChildEntity()
                }
            });
            context.SaveChanges();

            var baseQuery = context.RootEntities
                .Include(r => r.ParentEntity)
                .Include(r => r.Children);

            var queries = new Dictionary<string, IQueryable<RootEntity>>
            {
                { "lambdaQuery", baseQuery.OrderBy(r => r.ParentEntity == null ? "" : r.ParentEntity.Name).ThenBy(r => r.Name) },

                { "dynamicQuery", baseQuery.OrderBy("ParentEntity == null ? \"\" : ParentEntity.Name ASC, Name ASC") },

                { "castedLambdaQuery", baseQuery.OrderBy(r => (object)r.ParentEntity == null ? "" : r.ParentEntity.Name).ThenBy(r => r.Name) }
            };

            foreach (var query in queries)
            {
                Console.WriteLine($"{query.Key}:\n {GetDebugView(query.Value.Expression)}");
                Console.WriteLine("\n\n");
            }

            foreach (var query in queries)
            {
                try
                {
                    query.Value.ToList();
                    Console.WriteLine($"{query.Key} didn't throw\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{query.Key} threw: {ex}");
                }
                Console.WriteLine(Environment.NewLine);
            }

            Console.ReadKey();
        }
    }

    class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CoalesceGithubIssue25;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }

        public DbSet<ParentEntity> ParentEntities { get; set; }
        public DbSet<RootEntity> RootEntities { get; set; }
        public DbSet<ChildEntity> ChildEntities { get; set; }
    }

    class ParentEntity
    {
        public int ParentEntityId { get; set; }

        public string Name { get; set; }
    }

    class RootEntity
    {
        public int RootEntityId { get; set; }

        public string Name { get; set; }

        public int? ParentEntityId { get; set; }
        public ParentEntity ParentEntity { get; set; }

        public ICollection<ChildEntity> Children { get; set; }
    }

    class ChildEntity
    {
        public int ChildEntityId { get; set; }

        public int RootEntityId { get; set; }
        public RootEntity RootEntity { get; set; }
    }
}

using NFluent;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Entities;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void OfType_Dynamic()
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };
#if NETCOREAPP
            config.CustomTypeProvider = new NetStandardCustomTypeProvider();
#endif

            var qry = new[]
            {
                new CompanyWithBaseEmployees
                {
                    Employees = new BaseEmployee[]
                    {
                        new Worker { Name = "e" }, new Boss { Name = "e" }
                    }
                }
            }.AsQueryable();

            // Act
            var oftype = qry.Select(c => c.Employees.OfType<Worker>().Where(e => e.Name == "e")).ToArray();
            var oftypeDynamic = qry.Select(config, "Employees.OfType(\"System.Linq.Dynamic.Core.Tests.Entities.Worker\").Where(Name == \"e\")").ToDynamicArray();

            // Assert
            Check.That(oftypeDynamic.Length).Equals(oftype.Length);
        }

        [Fact]
        public void CastToType_Dynamic()
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };
#if NETCOREAPP
            config.CustomTypeProvider = new NetStandardCustomTypeProvider();
#endif

            var qry = new[]
            {
                new CompanyWithBaseEmployees
                {
                    Employees = new BaseEmployee[]
                    {
                        new Worker { Name = "e" }
                    }
                }
            }.AsQueryable();

            // Act
            var cast = qry.Select(c => c.Employees.Cast<Worker>().Where(e => e.Name == "e")).ToArray();
            var castDynamic = qry.Select(config, "Employees.Cast(\"System.Linq.Dynamic.Core.Tests.Entities.Worker\").Where(Name == \"e\")").ToDynamicArray();

            // Assert
            Check.That(cast.Length).Equals(castDynamic.Length);
        }

        [Fact]
        public void OfType_Dynamic_Exceptions()
        {
            // Assign
            var qry = new[]
            {
                new CompanyWithBaseEmployees
                {
                    Employees = new BaseEmployee[]
                    {
                        new Worker { Name = "e" }, new Boss { Name = "e" }
                    }
                }
            }.AsQueryable();

            // Act
            Assert.Throws<ParseException>(() => qry.Select("Employees.OfType().Where(Name == \"e\")"));
            Assert.Throws<ParseException>(() => qry.Select("Employees.OfType(true).Where(Name == \"e\")"));
            Assert.Throws<ParseException>(() => qry.Select("Employees.OfType(\"not-found\").Where(Name == \"e\")"));
        }
    }
}

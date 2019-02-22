using NFluent;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Entities;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void OfType_WithType()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "e" }, new Boss { Name = "e" }
            }.AsQueryable();

            // Act
            var oftype = qry.OfType<Worker>().ToArray();
            var oftypeDynamic = qry.OfType(typeof(Worker)).ToDynamicArray();

            // Assert
            Check.That(oftypeDynamic.Length).Equals(oftype.Length);
        }

        [Fact]
        public void OfType_WithString()
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };
#if NETCOREAPP
            // config.CustomTypeProvider = new NetStandardCustomTypeProvider();
#endif
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "e" }, new Boss { Name = "e" }
            }.AsQueryable();

            // Act
            var oftype = qry.OfType<Worker>().ToArray();
            var oftypeDynamic = qry.OfType(config, typeof(Worker).FullName).ToDynamicArray();

            // Assert
            Check.That(oftypeDynamic.Length).Equals(oftype.Length);
        }

        [Fact]
        public void OfType_Dynamic()
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };
#if NETCOREAPP
            // config.CustomTypeProvider = new NetStandardCustomTypeProvider();
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

        [Theory]
        [InlineData("OfType")]
        [InlineData("is")]
        public void OfType_Dynamic_ActingOnIt(string function)
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };

            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Boss { Name = "b" }
            }.AsQueryable();

            // Act
            int countOfType = qry.Count(c => c is Worker);
            int countOfTypeDynamic = qry.Count(config, $"{function}(\"System.Linq.Dynamic.Core.Tests.Entities.Worker\")");

            // Assert
            Check.That(countOfTypeDynamic).Equals(countOfType);
        }

        [Fact]
        public void As_Dynamic_ActingOnIt()
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };

            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Boss { Name = "b" }
            }.AsQueryable();

            // Act
            int countAsDynamic = qry.Count(config, $"As(\"System.Linq.Dynamic.Core.Tests.Entities.Worker\") != null");

            // Assert
            Check.That(countAsDynamic).Equals(1);
        }

        [Fact]
        public void CastToType_WithType()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Worker { Name = "2" }
            }.AsQueryable();

            // Act
            var cast = qry.Cast<Worker>().ToArray();
            var castDynamic = qry.Cast(typeof(Worker)).ToDynamicArray();

            // Assert
            Check.That(castDynamic.Length).Equals(cast.Length);
        }

        [Fact]
        public void CastToType_WithString()
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };

            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Worker { Name = "2" }
            }.AsQueryable();

            // Act
            var cast = qry.Cast<Worker>().ToArray();
            var castDynamic = qry.Cast(config, typeof(Worker).FullName).ToDynamicArray();

            // Assert
            Check.That(castDynamic.Length).Equals(cast.Length);
        }

        [Fact]
        public void CastToType_Dynamic()
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };

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
        public void CastToType_Dynamic_ActingOnIt()
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };

            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Worker { Name = "2" }
            }.AsQueryable();

            // Act
            var cast = qry.Select(c => (Worker)c).ToArray();
            var castDynamic = qry.Select(config, "Cast(\"System.Linq.Dynamic.Core.Tests.Entities.Worker\")").ToDynamicArray();

            // Assert
            Check.That(cast.Length).Equals(castDynamic.Length);
        }

        [Fact]
        public void CastToType_Dynamic_ActingOnIt_Throws()
        {
            // Assign
            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true
            };

            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Boss { Name = "b" }
            }.AsQueryable();

            // Act
            Action castDynamic = () => qry.Select(config, "Cast(\"System.Linq.Dynamic.Core.Tests.Entities.Worker\")").ToDynamicArray();

            // Assert
            Check.ThatCode(castDynamic).Throws<Exception>();
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

        [Fact]
        public void OfType_Dynamic_ActingOnIt_Exceptions()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Boss { Name = "b" }
            }.AsQueryable();

            // Act
            Assert.Throws<ParseException>(() => qry.Count("OfType()"));
            Assert.Throws<ParseException>(() => qry.Count("OfType(true)"));
            Assert.Throws<ParseException>(() => qry.Count("OfType(\"not-found\")"));
        }

        [Fact]
        public void CastToType_Dynamic_Exceptions()
        {
            // Assign
            var qry = new[]
            {
                new CompanyWithBaseEmployees
                {
                    Employees = new BaseEmployee[]
                    {
                        new Worker { Name = "1" }, new Worker { Name = "2" }
                    }
                }
            }.AsQueryable();

            // Act
            Assert.Throws<ParseException>(() => qry.Select("Employees.Cast().Where(Name == \"1\")"));
            Assert.Throws<ParseException>(() => qry.Select("Employees.Cast(true).Where(Name == \"1\")"));
            Assert.Throws<ParseException>(() => qry.Select("Employees.Cast(\"not-found\").Where(Name == \"1\")"));
        }
    }
}

using ConsoleApp_net452_EF6.Entities;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ConsoleApp_net452_EF6
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ParsingConfig
            {
                UseParameterizedNamesInDynamicQuery = true
            };

            var q = new[] { 1, 2, 3 }.AsQueryable();
            var r = q.Where("it > 1").ToArray();

            using (var context = new KendoGridDbContext())
            {
                var found1 = context.Employees.FirstOrDefault(config, "EmployeeNumber > 1000");
                Console.WriteLine($"found1 : {found1.Id} - {found1.EmployeeNumber}");

                var found2 = context.Employees.FirstOrDefault(config, "EmployeeNumber > @0", 1001);
                Console.WriteLine($"found2 : {found2.Id} - {found2.EmployeeNumber}");

                int em = 1002;
                var found3 = context.Employees.FirstOrDefault(config, "EmployeeNumber > @0", em);
                Console.WriteLine($"found3 : {found3.Id} - {found3.EmployeeNumber}");

                var foundFirstName1 = context.Employees.FirstOrDefault(config, "FirstName.Contains(\"e\")");
                Console.WriteLine($"foundFirstName1 : {foundFirstName1.Id} - {foundFirstName1.FirstName}");

                var foundFirstName2 = context.Employees.FirstOrDefault(config, "FirstName.StartsWith(@0)", "J");
                Console.WriteLine($"foundFirstName2 : {foundFirstName2.Id} - {foundFirstName2.FirstName}");

                string ends = "h";
                var foundFirstName3 = context.Employees.FirstOrDefault(config, "FirstName.EndsWith(@0)", ends);
                Console.WriteLine($"foundFirstName3 : {foundFirstName3.Id} - {foundFirstName3.FirstName}");

                string search = "2";
                var expected = context.Employees.Where(e => System.Data.Entity.SqlServer.SqlFunctions.StringConvert((double)e.EmployeeNumber).Contains(search)).ToArray();
                foreach (var emp in expected)
                {
                    Console.WriteLine($"System.Linq : {emp.Id} - {emp.EmployeeNumber}");
                }

                var test = context.Employees.Where("SqlFunctions.StringConvert(double(it.EmployeeNumber)).Contains(@0)", search).ToArray();
                foreach (var emp in test)
                {
                    Console.WriteLine($"DynamicLinq : {emp.Id} - {emp.EmployeeNumber}");
                }

                Console.WriteLine(new string('-', 80));
                int x = 1002;
                int seven = 7;
                var testNonOptimize = context.Employees.Where(e => e.EmployeeNumber > 1000 && e.EmployeeNumber < x && 7 == seven);
                Console.WriteLine($"testNonOptimize {testNonOptimize}");
                foreach (var emp in testNonOptimize)
                {
                    Console.WriteLine($"testNonOptimize: {emp.Id} - {emp.EmployeeNumber}");
                }

                var testNonOptimizeDynamic = context.Employees.Where("EmployeeNumber > 1000 && EmployeeNumber < @0 and 7 == @1 and \"2\" == @2", x, seven, search);
                Console.WriteLine($"testNonOptimizeDynamic {testNonOptimizeDynamic}");
                foreach (var emp in testNonOptimizeDynamic)
                {
                    Console.WriteLine($"testNonOptimizeDynamic: {emp.Id} - {emp.EmployeeNumber}");
                }

                Console.WriteLine("Enable ExpressionOptimizer.visit");
                ExtensibilityPoint.QueryOptimizer = ExpressionOptimizer.visit;

                //var testOptimize1 = context.Employees.Where(e => e.EmployeeNumber > 1000 && e.EmployeeNumber < x && 7 == seven);
                //var expression1 = ExpressionOptimizer.visit(testOptimize1.Expression);
                //Console.WriteLine(expression1);

                var testOptimizeDynamic = context.Employees.Where("EmployeeNumber > 1000 && EmployeeNumber < @0 and 7 == @1 and \"2\" == @2", x, seven, search);
                Console.WriteLine($"testOptimizeDynamic : {testOptimizeDynamic}");
                foreach (var emp in testOptimizeDynamic)
                {
                    Console.WriteLine($"testOptimizeDynamic: {emp.Id} - {emp.EmployeeNumber}");
                }
            }
        }
    }
}

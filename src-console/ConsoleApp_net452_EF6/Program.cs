using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp_net452_EF6.Entities;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;

namespace ConsoleApp_net452_EF6
{
    class Program
    {
        static void Main(string[] args)
        {
            var all = new
            {
                test1 = new List<int> { 1, 2, 3 }.ToDynamicList(typeof(int)),
                test2 = new List<dynamic> { 4, 5, 6 }.ToDynamicList(typeof(int)),
                test3 = new List<object> { 7, 8, 9 }.ToDynamicList(typeof(int))
            };

            Console.WriteLine("all {0}", JsonConvert.SerializeObject(all, Formatting.Indented));
            using (var context = new KendoGridDbContext())
            {
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

                Console.WriteLine(new String('-', 80));
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

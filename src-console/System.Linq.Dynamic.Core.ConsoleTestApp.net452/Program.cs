using System.Linq.Dynamic.Core.ConsoleTestApp.net452.Entities;

namespace System.Linq.Dynamic.Core.ConsoleTestApp.net452
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new KendoGridDbContext())
            {
                string search = "2";
                var expected = context.Employees.Where(e => System.Data.Entity.SqlServer.SqlFunctions.StringConvert((double) e.EmployeeNumber).Contains(search)).ToArray();
                foreach (var emp in expected)
                {
                    Console.WriteLine($"System.Linq : {emp.Id} - {emp.EmployeeNumber}");
                }

                var test = context.Employees.Where("SqlFunctions.StringConvert(double(it.EmployeeNumber)).Contains(@0)", search).ToArray();
                foreach (var emp in test)
                {
                    Console.WriteLine($"DynamicLinq : {emp.Id} - {emp.EmployeeNumber}");
                }
            }
        }
    }
}
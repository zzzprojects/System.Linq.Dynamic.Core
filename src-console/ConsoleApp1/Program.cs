using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace ConsoleApp1
{
    class Employee
    {
        public string Name { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var emps = new[] { new Employee { Name = "a" }, new Employee { Name = "b" } }.AsQueryable();

            // normal
            Expression<Func<Employee, string>> exp = u => u.Name;
            var equery1 = emps.Select(exp);
            Console.WriteLine("equery1 :" + string.Join(",", equery1));

            // lambdaExpression
            var lambdaExpression = DynamicExpressionParser.ParseLambda(false, typeof(Employee), null, "Name");
            var body = lambdaExpression.Body;
            var p = lambdaExpression.Parameters;
            var x = Expression.Lambda<Func<Employee, string>>(body, p);

            var equery2 = emps.Select(x);
            Console.WriteLine("equery2 :" + string.Join(",", equery2));
        }
    }
}
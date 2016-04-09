//using DynamicLinqWebDocs.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace DynamicLinqWebDocs.Infrastructure.Data
//{
//    class TestDataRepo : IDataRepo 
//    {
//        public static List<Class> Classes { get; private set; }

//        public static List<Expression> Expressions { get; private set; }

//        static TestDataRepo()
//        {
//            Classes = new List<Class>();
//            Expressions = new List<Expression>();

//            var basicQueryableClass = new Class()
//            {
//                Name = "BasicQueryable",
//                 Description = "Provides a set of **static** methods for querying data structures that implement `IQueryable`.",
//                 Namespace = "System.Linq",
//                 Remarks = "Test remarks",
//                 Methods = new List<Method>()
//            };

//            Classes.Add(basicQueryableClass);

//            var anyMethod = new Method()
//            {
//                Name = "Any",
//                Description = "Determines whether a sequence contains any elements.",
//                Remarks = "The query behavior that occurs as a result of executing an expression tree that represents calling `Any<TSource>(IQueryable<TSource>)` depends on the implementation of the type of the source parameter. The expected behavior is that it determines if source contains any elements.\n\nThis is very similar to `Queryable.Any()`, except that the standard `Any()` method uses `IQueryable<T>`.",
//                Examples = new List<Example>(),
//                Arguments = new List<Argument>(),
//                IsStatic = true,
//                IsExtensionMethod = true,
//            };

//            anyMethod.Arguments.Add(new Argument()
//            {
//                Type = "IQueryable",
//                Name = "source",
//                TypeNamespace = "System.Linq",
//                Description = "A sequence to check for being empty."
//            });

//            anyMethod.Examples.Add(new Example()
//            {
//                HeaderRemarks = "The following code example demonstrates how to use `Any<TSource>(IQueryable<TSource>)` to determine whether a sequence contains any elements.",
//                FooterRemarks = "The Boolean value that the `Any<TSource>(IQueryable<TSource>)` method returns is typically used in the predicate of a where clause (Where clause in Visual Basic) or a direct call to the `Where<TSource>(IQueryable<TSource>, Expression<Func<TSource, Boolean>>)` method. The following example demonstrates this use of the Any method.",
//                ExampleCode = "List<int> numbers = new List<int> { 1, 2 };\n\n// Determine if the list contains any elements.\nbool hasElements = numbers.AsQueryable().Any();"
//            });

//            basicQueryableClass.Methods.Add(anyMethod);


//            var expression = new Expression()
//            {
//                Name = "Where",
//                Description = "Where Description",
//                Remarks = "Where Remarks",
//                Examples = new List<Example>()
//            };

//            expression.Examples.Add(new Example()
//            {
//                 HeaderRemarks = "Simple Where Expression",
//                 ExampleCode = "var userById = qry.Where(\"Id=@0\", SomeId);",
//                 FooterRemarks = "Nice example!"
//            });

//            Expressions.Add(expression);

//        }

//        public Class GetClass(string className)
//        {
//            return Classes.Where(x => className.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
//        }

//        public Method GetMethod(string @className, string methodName, out Class @class)
//        {
//            @class = GetClass(@className);

//            if (@class == null) return null;

//            return @class.Methods
//                .Where(x => methodName.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))
//                .FirstOrDefault();
//        }

//        public Expression GetExpression(string name)
//        {
//            return Expressions
//                .Where(x => name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))
//                .FirstOrDefault();
//        }


//        public IEnumerable<Class> GetClasses()
//        {
//            return Classes;
//        }


//        public IEnumerable<Expression> GetExpressions()
//        {
//            return Expressions;
//        }
//    }
//}
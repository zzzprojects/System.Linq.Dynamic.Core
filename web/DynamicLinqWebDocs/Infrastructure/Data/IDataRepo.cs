using System.Collections.Generic;
using DynamicLinqWebDocs.Models;

namespace DynamicLinqWebDocs.Infrastructure.Data
{
    public interface IDataRepo
    {
        IEnumerable<Class> GetClasses();

        Class GetClass(string className);

        Method GetMethod(string className, string methodName, Frameworks framework, out Class @class, int overload = 0);

        Property GetProperty(string className, string propertyName, Frameworks framework, out Class @class);


        IEnumerable<Expression> GetExpressions();

        Expression GetExpression(string expressionName);


        IEnumerable<Keyword> GetKeywords();

        Keyword GetKeyword(string keywordName);
    }
}

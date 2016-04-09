using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace DynamicLinqWebDocs.Infrastructure
{
    public class EnumRouteConstraint : IRouteConstraint
    {
        readonly HashSet<string> _enumValues;

        public EnumRouteConstraint(string enumTypeName)
        {
            var enumType = Assembly.GetExecutingAssembly().GetType(enumTypeName);

            _enumValues = new HashSet<string>(Enum.GetNames(enumType).Select(x => x.ToLower()));
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var currentValue = values[parameterName].ToString().ToLower();

            var result = _enumValues.Contains(currentValue);

            return result;
        }
    }
}
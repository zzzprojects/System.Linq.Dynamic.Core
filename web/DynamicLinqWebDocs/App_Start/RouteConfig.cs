using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;
using DynamicLinqWebDocs.Infrastructure;

namespace DynamicLinqWebDocs
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var constraintsResolver = new DefaultInlineConstraintResolver();
            constraintsResolver.ConstraintMap.Add("Enum", typeof(EnumRouteConstraint));

            routes.MapMvcAttributeRoutes(constraintsResolver);


            //routes.MapRoute(
            //    name: "",
            //    url: "docs/expression/{name}",
            //    defaults: new { controller = "Docs", action = "Expression" }
            //    );



            //routes.MapRoute(
            //    name: "",
            //    url: "Classes",
            //    defaults: new { controller = "Class", action = "Index" }
            //    );

            //routes.MapRoute(
            //    name: "",
            //    url: "Classes/{className}/{methodName}/{framework}",
            //    defaults: new { controller = "Class", action = "ClassMethod", methodName = UrlParameter.Optional, framework = Frameworks.NotSet }
            //    );

            //routes.MapRoute(
            //    name: "",
            //    url: "Expressions",
            //    defaults: new { controller = "Expression", action = "Index" }
            //    );

            //routes.MapRoute(
            //    name: "",
            //    url: "Expressions/{expressionName}",
            //    defaults: new { controller = "Expression", action = "Expression" }
            //    );

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //    );
        }
    }
}

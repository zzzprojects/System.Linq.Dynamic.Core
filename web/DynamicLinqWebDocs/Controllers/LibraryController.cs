using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicLinqWebDocs.Infrastructure;
using DynamicLinqWebDocs.Infrastructure.Data;
using DynamicLinqWebDocs.Models;
using SimpleMvcSitemap;
using Class = DynamicLinqWebDocs.ViewModels.Class;
using Method = DynamicLinqWebDocs.ViewModels.Method;
using Property = DynamicLinqWebDocs.ViewModels.Property;

namespace DynamicLinqWebDocs.Controllers
{
    [RoutePrefix("Library")]
    public class LibraryController : Controller
    {
        readonly IDataRepo _repo = new RealDataRepo();

        [Route]
        public ActionResult Index()
        {
            this.SetMetaDescription("List of classes and interfaces inside the Dynamic LINQ Library.");
            this.AddMetaKeywords("References");

            return View(_repo.GetClasses());
        }

        [Route("{classname}")]
        public ActionResult Class(string className)
        {
            var @class = _repo.GetClass(className);
            if (@class == null) return HttpNotFound();

            var entityName = @class.IsInterface ? "Interface" : "Class";

            this.SetMetaDescription("A list of methods and properties defined in the {0} {1}.", @class.Name, entityName.ToLower());
            this.AddMetaKeywords(entityName, @class.Name);

            var viewModel = new Class()
            {
                Name = @class.Name,
                Namespace = @class.Namespace,
                Description = @class.Description,
                Remarks = @class.Remarks,
                Methods = @class.Methods,
                Properties = @class.Properties,
                IsInterface = @class.IsInterface 
            };

            return View(viewModel);
        }

        [Route("{className}/{methodName}/{framework:Enum(DynamicLinqWebDocs.Models.Frameworks)?}")]
        //
        // GET: /Docs/
        public ActionResult Method(string className, string methodName, Frameworks framework = Frameworks.NotSet, int o = 0)
        {
            Models.Class @class;

            var formattedMethodName = methodName.Replace('(', '<').Replace(')', '>');

            var method = _repo.GetMethod(className, formattedMethodName, framework, out @class, o);
            if (method == null) return HttpNotFound();

            this.SetMetaDescription("The syntax and description of the {0}.{1} method.", @class.Name, formattedMethodName);
            this.AddMetaKeywords("Method", @class.Name, method.Name);

            var viewModel = new Method()
            {
                Namespace = @class.Namespace,
                Class = @class.Name,
                Name = method.Name,
                Arguments = method.Arguments,
                IsStatic = method.IsStatic,
                IsExtensionMethod = method.IsExtensionMethod,
                ReturnType = method.ReturnType,
                Remarks = method.Remarks,
                Description = method.Description,
                Examples = method.Examples,
                ReturnDescription = method.ReturnDescription,
                Frameworks = method.Frameworks,
                HasParamsArgument = method.HasParamsArgument
            };

            return View(viewModel);
        }

        [Route("{className}/Property-{propertyName}/{framework:Enum(DynamicLinqWebDocs.Models.Frameworks)?}")]
        public ActionResult Property(string className, string propertyName, Frameworks framework = Frameworks.NotSet)
        {
            Models.Class @class;

            var property = _repo.GetProperty(className, propertyName, framework, out @class);
            if (property == null) return HttpNotFound();

            this.SetMetaDescription("The syntax and description of the {0}.{1} property.", @class.Name, property.Name);
            this.AddMetaKeywords("Property", @class.Name, property.Name);

            var viewModel = new Property()
            {
                Namespace = @class.Namespace,
                Class = @class.Name,
                Name = property.Name,
                IsStatic = property.IsStatic,
                ValueType = property.ValueType,
                Remarks = property.Remarks,
                Description = property.Description,
                Examples = property.Examples,
                ValueTypeDescription = property.ValueTypeDescription,
                Frameworks = property.Frameworks,
                Accessors = property.Accessors
            };

            return View(viewModel);
        }


        class LibrarySitemap : SitemapContributor
        {
            protected internal override IEnumerable<SitemapNode> GetSitemapNodes(UrlHelper urlHelper, HttpContextBase httpContext)
            {
                yield return new SitemapNode(urlHelper.Action("Index", "Library")) { Priority = 0.8m };

                var repo = new RealDataRepo();

                foreach (var @class in repo.GetClasses())
                {
                    yield return new SitemapNode(urlHelper.Action("Class", "Library", new { className = @class.Name })) { Priority = 0.75m };

                    foreach (var methodGrp in @class.Methods.GroupBy(x => new { x.Name, x.Frameworks }))
                    {
                        int methodCount = 0;

                        foreach (var method in methodGrp)
                        {
                            Frameworks? framework = null;

                            if (method.Frameworks != Frameworks.All)
                            {
                                framework = Enum.GetValues(typeof(Frameworks)).Cast<Frameworks>().Reverse().Where(x => method.Frameworks.HasFlag(x)).FirstOrDefault();
                            }

                            var methodUrl = urlHelper.Action("Method", "Library", new { className = @class.Name, methodName = method.Name.Replace('<', '(').Replace('>', ')'), framework = framework, o = methodCount > 0 ? (int?)methodCount : null }, null);

                            yield return new SitemapNode(methodUrl) { Priority = .5m };

                            methodCount++;
                        }
                    }

                    foreach (var property in @class.Properties)
                    {
                        Frameworks? framework = null;

                        if (property.Frameworks != Frameworks.All)
                        {
                            framework = Enum.GetValues(typeof(Frameworks)).Cast<Frameworks>().Reverse().Where(x => property.Frameworks.HasFlag(x)).FirstOrDefault();
                        }

                        var propertyUrl = urlHelper.Action("Property", "Library", new { className = @class.Name, propertyName = property.Name, framework = framework });

                        yield return new SitemapNode(propertyUrl) { Priority = .5m };
                    }
                }
            }
        }
	}
}
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using DynamicLinqWebDocs.Infrastructure;
using DynamicLinqWebDocs.Infrastructure.Data;
using SimpleMvcSitemap;

namespace DynamicLinqWebDocs.Controllers
{
    public class ExpressionController : Controller
    {
        readonly IDataRepo _repo = new RealDataRepo();

        [Route("Expressions")]
        public ActionResult Index()
        {
            this.SetMetaDescription("List of expression methods that can be used inside a Dynamic LINQ string expression.");
            this.AddMetaKeywords("Expressions");

            return View(_repo);
        }

        [Route("Expressions/{expressionName}")]
        public ActionResult Expression(string expressionName) 
        {
            var expression = _repo.GetExpression(expressionName);
            if (expression == null) return HttpNotFound();

            this.SetMetaDescription("The syntax and description of the {0} expression method.", expression.Name);
            this.AddMetaKeywords("Expression Method", expression.Name);

            return View(expression);
        }

        [Route("Keywords/{keywordName}")]
        public ActionResult Keyword(string keywordName)
        {
            var keyword = _repo.GetKeyword(keywordName);
            if (keyword == null) return HttpNotFound();

            this.SetMetaDescription("The description of the {0} keyword.", keyword.Name);
            this.AddMetaKeywords("Keyword", keyword.Name);

            return View(keyword);
        }

        class ExpressionSitemap : SitemapContributor
        {
            protected internal override IEnumerable<SitemapNode> GetSitemapNodes(UrlHelper urlHelper, HttpContextBase httpContext)
            {
                yield return new SitemapNode(urlHelper.Action("Index", "Expression")) { Priority = 0.8m };

                var repo = new RealDataRepo();

                foreach (var expression in repo.GetExpressions())
                {
                    yield return new SitemapNode(urlHelper.Action("Expression", "Expression", new { expressionName = expression.Name })) { Priority = 0.50m };
                }

                foreach (var keyword in repo.GetKeywords())
                {
                    yield return new SitemapNode(urlHelper.Action("Keyword", "Expression", new { keywordName = keyword.Name })) { Priority = 0.50m };
                }
            }
        }
	}
}
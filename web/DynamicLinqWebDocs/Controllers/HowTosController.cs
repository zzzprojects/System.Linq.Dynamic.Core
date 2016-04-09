using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using DynamicLinqWebDocs.Infrastructure;
using SimpleMvcSitemap;

namespace DynamicLinqWebDocs.Controllers
{
    [RoutePrefix("HowTos")]
    public class HowTosController : Controller
    {
        [Route()]
        public ActionResult Index()
        {
            return View();
        }

        [Route("EF6")]
        // GET: HowTos
        public ActionResult EF6()
        {
            return View();
        }

        class HowTosSitemap : SitemapContributor
        {
            protected internal override IEnumerable<SitemapNode> GetSitemapNodes(UrlHelper urlHelper, HttpContextBase httpContext)
            {
                yield return new SitemapNode(urlHelper.Action("Index", "HowTos")) { Priority = 0.3m };
                yield return new SitemapNode(urlHelper.Action("EF6", "HowTos")) { Priority = 0.4m };
            }
        }
    }
}
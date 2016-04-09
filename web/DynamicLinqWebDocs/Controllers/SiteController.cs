using System.Web.Mvc;
using DynamicLinqWebDocs.Infrastructure;

namespace DynamicLinqWebDocs.Controllers
{

    public class SiteController : Controller
    {

        [Route("sitemap.xml")]
        public ActionResult SitemapXml()
        {
            return SitemapGenerator.GetSitemapXml(Url, HttpContext);
        }

    }
}
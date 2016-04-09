using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DynamicLinqWebDocs.Controllers
{
    /// <summary>
    /// Temporary perminent redirect for all /classes urls since it was renamed to library.
    /// </summary>
    [RoutePrefix("Classes")]
    public class ClassController : Controller
    {
        [Route("{*values}")]
        
        public ActionResult Any()
        {
            var url = Request.Url.ToString();

            var newUrl = Regex.Replace(url, "(classes)", "Library", RegexOptions.IgnoreCase);

            return RedirectPermanent(newUrl);
        }
    }
}
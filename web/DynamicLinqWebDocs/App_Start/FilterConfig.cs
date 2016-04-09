using System.Web.Mvc;

namespace DynamicLinqWebDocs
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

#if !DEBUG
            //Cache all pages for 15 minutes
            filters.Add(new OutputCacheAttribute() { Duration = 900 }); 
#endif
        }
    }
}

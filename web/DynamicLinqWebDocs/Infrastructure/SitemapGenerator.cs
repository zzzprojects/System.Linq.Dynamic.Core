using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using SimpleMvcSitemap;

namespace DynamicLinqWebDocs.Infrastructure
{
    public static class SitemapGenerator
    {
        static ActionResult _sitemap;

        public static ActionResult GetSitemapXml(UrlHelper url, HttpContextBase httpContext)
        {
            if (_sitemap == null)
            {
                var nodes = SitemapContributor.GetContributors().SelectMany(x => x.GetSitemapNodes(url, httpContext)).ToArray();

                foreach( var node in nodes.Where( x => !x.ChangeFrequency.HasValue ))
                {
                    node.ChangeFrequency = ChangeFrequency.Daily;
                }

                _sitemap = new SitemapProvider().CreateSitemap(httpContext, nodes);
            }

            return _sitemap;
        }
    }
    
    public abstract class SitemapContributor
    {
        internal protected abstract IEnumerable<SitemapNode> GetSitemapNodes(UrlHelper urlHelper, HttpContextBase httpContext);

        internal static IEnumerable<SitemapContributor> GetContributors()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.IsSubclassOf(typeof(SitemapContributor)))
                .Select(x => (SitemapContributor)Activator.CreateInstance(x));
        }
    }
        
}
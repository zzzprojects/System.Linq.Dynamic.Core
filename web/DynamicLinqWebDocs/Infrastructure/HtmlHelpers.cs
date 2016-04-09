using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MarkdownSharp;

namespace DynamicLinqWebDocs.Infrastructure
{
    public static class HtmlHelpers
    {
        public static HtmlString FormatMarkdown(this HtmlHelper helper, string value)
        {
            return FormatMarkdown(value);
        }

        public static HtmlString FormatMarkdown(this HtmlHelper helper, string format, params object[] args)
        {
            return FormatMarkdown(String.Format(format, args));
        }

        public static HtmlString FormatCodeBlock(this HtmlHelper helper, string value)
        {
            var pre = new TagBuilder("pre");

            pre.AddCssClass("sunlight-highlight-csharp");

            pre.SetInnerText(value);

            return new HtmlString(pre.ToString());
        }

        static HtmlString FormatMarkdown(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;

            var md = new Markdown();

            //if (convertToCode) value = ConvertToCode(value);

            var result = md.Transform(value);

            return new HtmlString(result);
        }

        public static HtmlString InlineCodeList(this HtmlHelper helper, params string[] items)
        {
            var ul = new TagBuilder("ul");
            ul.AddCssClass("list-inline");

            var sb = new StringBuilder();

            foreach(var item in items)
            {
                sb.AppendFormat("<li><code>{0}</code></li>", item);
            }

            ul.InnerHtml = sb.ToString();

            return new HtmlString(ul.ToString());
        }


        public static IDisposable BeginNote(this HtmlHelper helper)
        {
            return new HtmlBootstrapNote(helper.ViewContext);
        }

        class HtmlBootstrapNote : IDisposable
        {
            readonly TextWriter _writer;

            public HtmlBootstrapNote(ViewContext context)
            {
                _writer = context.Writer;

                _writer.WriteLine("<div class=\"panel panel-default\"><div class=\"panel-heading\">Note</div><div class=\"panel-body\">");
            }

            public void Dispose()
            {
                _writer.WriteLine("</div></div>");
            }
        }

        public static HtmlString IsActive(this HtmlHelper html, string url)
        {
            return IsActive(html, url, false);
        }

        public static HtmlString IsActive(this HtmlHelper html, string url, bool startsWith)
        {
            var request = html.ViewContext.RequestContext.HttpContext.Request.Url;

            bool isActive;

            if (startsWith)
            {
                isActive = request.AbsolutePath.StartsWith(url, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                isActive = request.AbsolutePath.Equals(url, StringComparison.InvariantCultureIgnoreCase);
            }

            return isActive ? new HtmlString("active") : null;
        }

        //static string ConvertToCode(string value)
        //{
        //    var sb = new StringBuilder();

        //    if (value.Length > 0) sb.Append("    ");

        //    for( int i = 0; i < value.Length; i++)
        //    {
        //        sb.Append(value[i]);

        //        //only add space if we're not at the end
        //        if (value[i] == '\n' && i != value.Length - 1)
        //        {
        //            sb.Append("    ");
        //        }
        //    }

        //    return sb.ToString();
        //}

    }
}
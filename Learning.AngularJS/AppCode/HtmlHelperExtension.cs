using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using Learn.AngularJS;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtension
    {
        public static MvcHtmlString Script(this HtmlHelper html, string contentPath)
        {
            return VersionedContent(html, "<script src=\"{0}\" type=\"text/javascript\"></script>", contentPath);
        }

        public static MvcHtmlString Style(this HtmlHelper html, string contentPath)
        {
            return VersionedContent(html, "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\">", contentPath);
        }

        private static MvcHtmlString VersionedContent(this HtmlHelper html, string template, string contentPath)
        {
            contentPath = UrlHelper.GenerateContentUrl(contentPath, html.ViewContext.HttpContext) + "?v=" + VersionUtils.VersionNumber;
            return MvcHtmlString.Create(string.Format(template, contentPath));
        }
    }
}
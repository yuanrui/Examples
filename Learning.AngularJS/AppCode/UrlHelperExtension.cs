using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Learn.AngularJS;

namespace System.Web.Mvc
{
    public static class UrlHelperExtension
    {
        public static string ContentVersioned(this UrlHelper urlHelper, string contentPath)
        {
            return String.Format("{0}?v={1}", urlHelper.Content(contentPath), VersionUtils.VersionNumber);
        }

    }
}
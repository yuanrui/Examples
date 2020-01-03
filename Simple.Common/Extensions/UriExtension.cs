using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Common.Extensions
{
    public static class UriExtension
    {
        public static string GetBaseUrl(this Uri uri)
        {
            return uri.Scheme + "://" + uri.Host + ":" + uri.Port;
        }
    }
}

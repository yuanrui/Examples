using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Learn.AngularJS
{
    public static class VersionUtils
    {
        public readonly static DateTime VersionDate;

        public readonly static Int64 VersionNumber;

        static VersionUtils()
        {
            VersionDate = System.IO.File.GetLastWriteTime(typeof(VersionUtils).Assembly.Location);
            VersionNumber = Int64.Parse(VersionDate.ToString("yyyyMMddHHmm"));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Common.Utility
{
    public class VersionUtils
    {
        public readonly static DateTime VersionDate;

        public readonly static Int64 VersionNumber;

        static VersionUtils()
        {
            VersionDate = VersionUtils<VersionUtils>.VersionDate;
            VersionNumber = VersionUtils<VersionUtils>.VersionNumber;
        }
    }

    public static class VersionUtils<TypeInAssembly>
    {
        public readonly static DateTime VersionDate;

        public readonly static Int64 VersionNumber;

        static VersionUtils()
        {
            VersionDate = System.IO.File.GetLastWriteTime(typeof(TypeInAssembly).Assembly.Location);
            VersionNumber = Int64.Parse(VersionDate.ToString("yyyyMMddHHmm"));
        }
    }
}

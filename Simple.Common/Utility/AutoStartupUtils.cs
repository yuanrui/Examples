using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Win32;

namespace Simple.Common.Utility
{
    public static class AutoStartupUtils
    {
        const string RegistrySubKey = @"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        static readonly string _currentAppPath;

        static AutoStartupUtils()
        {
            _currentAppPath = Assembly.GetEntryAssembly().Location;
        }

        public static void Set(string appName)
        {
            Set(appName, _currentAppPath, true);
        }

        public static void Set(string appName, bool enable)
        {
            Set(appName, _currentAppPath, enable);
        }

        public static void Set(string appName, string appPath, bool enable)
        {
            try
            {
                using (var runRegistryKey = Registry.CurrentUser.OpenSubKey(RegistrySubKey, true))
                {
                    if (runRegistryKey != null)
                    {
                        if (enable)
                        {
                            runRegistryKey.SetValue(appName, appPath);
                            Trace.WriteLine(string.Format("Program:[{0}] enable auto startup.", appName));
                        }
                        else
                        {
                            runRegistryKey.DeleteValue(appName, false);
                            Trace.WriteLine(string.Format("Program:[{0}] disable auto startup.", appName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0}[{1}]({2}) set auto startup exception:{3}", enable ? "enable" : "disable", appName, appPath, ex));
            }
        }
    }
}

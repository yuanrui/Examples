using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Win32;

namespace Study.CustomUrlScheme
{
    public static class WinUrlProtocolHelper
    {
        public static void Register(String protocol, String exePath, String description)
        {
            if (String.IsNullOrEmpty(protocol))
            {
                throw new ArgumentNullException("protocol");
            }

            if (String.IsNullOrEmpty(exePath))
            {
                throw new ArgumentNullException("exePath");
            }
            
            String protocolValue = description;
            if (String.IsNullOrEmpty(protocolValue))
            {
                protocolValue = protocol + " Protocol";
            }
            
            String rootKey = @"HKEY_CLASSES_ROOT\" + protocol;
            Registry.SetValue(
                rootKey,
                String.Empty,
                protocolValue,
                RegistryValueKind.String);
            Registry.SetValue(
                rootKey,
                "URL Protocol",
                String.Empty,
                RegistryValueKind.String);

            String command = String.Format("\"{0}\" \"%1\"", exePath);
            Registry.SetValue(rootKey + @"\shell\open\command", String.Empty, command, RegistryValueKind.String);
        }

        public static void UnRegister(String protocol)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot;
                key.DeleteSubKeyTree(protocol);
                key.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("delete HKEY_CLASSES_ROOT\\" + protocol + " Exception:" + ex.ToString());
            }            
        }
    }
}

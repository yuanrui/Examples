using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Security.Cryptography;

namespace Simple.Common.Utility
{
    public class MachineInfoUtils
    {
        public static string GetIPAddress()
        {
            string result = "";
            ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection instances = managementClass.GetInstances();
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ManagementObject managementObject = (ManagementObject)enumerator.Current;
                    if ((bool)managementObject["IPEnabled"])
                    {
                        Array array = (Array)managementObject.Properties["IpAddress"].Value;
                        result = array.GetValue(0).ToString();
                        break;
                    }
                }
            }
            return result;
        }

        public static string GetOSName()
        {
            string result = "";
            OperatingSystem oSVersion = Environment.OSVersion;
            switch (oSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                    {
                        int minor = oSVersion.Version.Minor;
                        if (minor != 0)
                        {
                            if (minor != 10)
                            {
                                if (minor == 90)
                                {
                                    result = "Windows Me";
                                }
                            }
                            else if (oSVersion.Version.Revision.ToString() == "2222A ")
                            {
                                result = "Windows 98 第二版";
                            }
                            else
                            {
                                result = "Windows 98";
                            }
                        }
                        else
                        {
                            result = "Windows 95";
                        }
                        break;
                    }
                case PlatformID.Win32NT:
                    switch (oSVersion.Version.Major)
                    {
                        case 3:
                            result = "Windows NT 3.51";
                            break;
                        case 4:
                            result = "Windows NT 4.0";
                            break;
                        case 5:
                            switch (oSVersion.Version.Minor)
                            {
                                case 0:
                                    result = "Windows 2000";
                                    break;
                                case 1:
                                    result = "Windows XP";
                                    break;
                                case 2:
                                    result = "Windows 2003";
                                    break;
                            }
                            break;
                        case 6:
                            switch (oSVersion.Version.Minor)
                            {
                                case 0:
                                    result = "Windows Vista";
                                    break;
                                case 1:
                                    result = "Windows 7";
                                    break;
                                case 2:
                                    result = "Windows 8";
                                    break;
                                case 3:
                                    result = "Windows 8.1";
                                    break;
                            }
                            break;
                        case 10:
                            {
                                int minor2 = oSVersion.Version.Minor;
                                if (minor2 == 0)
                                {
                                    result = "Windows 10";
                                }
                                break;
                            }
                    }
                    break;
            }
            return result;
        }

        public static string GetCpuId()
        {
            string text = identifier("Win32_Processor", "UniqueId");
            if (text == "")
            {
                text = identifier("Win32_Processor", "ProcessorId");
                if (text == "")
                {
                    text = identifier("Win32_Processor", "Name");
                    if (text == "")
                    {
                        text = identifier("Win32_Processor", "Manufacturer");
                    }
                    text += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return text;
        }

        public static string GetBiosId()
        {
            return string.Concat(new string[]
			{
				identifier("Win32_BIOS", "Manufacturer"),
				identifier("Win32_BIOS", "SMBIOSBIOSVersion"),
				identifier("Win32_BIOS", "IdentificationCode"),
				identifier("Win32_BIOS", "SerialNumber"),
				identifier("Win32_BIOS", "ReleaseDate"),
				identifier("Win32_BIOS", "Version")
			});
        }

        public static string GetDiskId()
        {
            return identifier("Win32_DiskDrive", "Model") + identifier("Win32_DiskDrive", "Manufacturer") + identifier("Win32_DiskDrive", "Signature") + identifier("Win32_DiskDrive", "TotalHeads");
        }

        public static string GetBaseId()
        {
            return identifier("Win32_BaseBoard", "Model") + identifier("Win32_BaseBoard", "Manufacturer") + identifier("Win32_BaseBoard", "Name") + identifier("Win32_BaseBoard", "SerialNumber");
        }

        public static string GetVideoId()
        {
            return identifier("Win32_VideoController", "DriverVersion") + identifier("Win32_VideoController", "Name");
        }

        public static string GetMacId()
        {
            return identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }

        public static string GetClientId()
        {
            return GetHash(string.Concat(new string[]
			{
                "<Company Info>",
				"\nCPU >> ",
				GetCpuId(),
				"\nBIOS >> ",
				GetBiosId(),
				"\nBASE >> ",
				GetBaseId(),
				"\nDISK >> ",
				GetDiskId(),
				"\nVIDEO >> ",
				GetVideoId(),
				"\nMAC >> ",
				GetMacId()
			}));
        }

        private static string GetHash(string s)
        {
            MD5 mD = new MD5CryptoServiceProvider();
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
            byte[] bytes = aSCIIEncoding.GetBytes(s);
            return GetHexString(mD.ComputeHash(bytes));
        }

        private static string GetHexString(byte[] bt)
        {
            string text = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int num = (int)b;
                int num2 = num & 15;
                int num3 = num >> 4 & 15;
                if (num3 > 9)
                {
                    text += ((char)(num3 - 10 + 65)).ToString();
                }
                else
                {
                    text += num3.ToString();
                }
                if (num2 > 9)
                {
                    text += ((char)(num2 - 10 + 65)).ToString();
                }
                else
                {
                    text += num2.ToString();
                }
                if (i + 1 != bt.Length && (i + 1) % 2 == 0)
                {
                    text += "-";
                }
            }
            return text;
        }

        private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string text = "";
            ManagementClass managementClass = new ManagementClass(wmiClass);
            ManagementObjectCollection instances = managementClass.GetInstances();
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ManagementObject managementObject = (ManagementObject)enumerator.Current;
                    if (managementObject[wmiMustBeTrue].ToString() == "True" && text == "")
                    {
                        try
                        {
                            text = managementObject[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return text;
        }

        private static string identifier(string wmiClass, string wmiProperty)
        {
            string text = "";
            ManagementClass managementClass = new ManagementClass(wmiClass);
            ManagementObjectCollection instances = managementClass.GetInstances();
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ManagementObject managementObject = (ManagementObject)enumerator.Current;
                    if (text == "")
                    {
                        try
                        {
                            text = managementObject[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return text;
        }
    }
}

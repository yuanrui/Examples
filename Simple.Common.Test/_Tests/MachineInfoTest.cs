using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Common.Utility;

namespace Simple.Common.Test._Tests
{
    public class MachineInfoTest
    {
        public static void Run()
        {
            Console.WriteLine("BaseId:" + MachineInfoUtils.GetBaseId());
            Console.WriteLine("IPAddress:" + MachineInfoUtils.GetIPAddress());
            Console.WriteLine("BiosId:" + MachineInfoUtils.GetBiosId());
            Console.WriteLine("CpuId:" + MachineInfoUtils.GetCpuId());
            Console.WriteLine("DiskId:" + MachineInfoUtils.GetDiskId());
            Console.WriteLine("MacId:" + MachineInfoUtils.GetMacId());
            Console.WriteLine("OSName:" + MachineInfoUtils.GetOSName());
            Console.WriteLine("VideoId:" + MachineInfoUtils.GetVideoId());
            Console.WriteLine("ClientId:" + MachineInfoUtils.GetClientId());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Common.Utility;
using System.Runtime.InteropServices;
using System.Threading;

namespace Simple.Common.Test._Tests
{
    public class StructureConverterTest
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class ClsDemo 
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime Time { get; set; }
            public long Token { get; set; }
            public double Value { get; set; }

            public ClsDemo()
            {
                Id = Guid.NewGuid().ToString();
                Time = DateTime.Now;
                Name = Time.ToString("yyyy-MM-dd");
                Token = Time.Ticks;
                Value = Time.TimeOfDay.TotalHours;
            }

            public override string ToString()
            {
                return "Id:" + Id + ", Name:" + Name + ", Time:" + Time.ToString() + ", Token:" + Token.ToString() + ", Value:" + Value.ToString();
            }
        }

        public struct StDemo 
        {
            public string Id;
            public string Name;
            public DateTime Time;
            public long Token;
            public double Value;

            public StDemo(DateTime now)
            {
                Id = Guid.NewGuid().ToString();
                Time = now;
                Name = Time.ToString("yyyy-MM-dd");
                Token = Time.Ticks;
                Value = Time.TimeOfDay.TotalHours;
            }

            public override string ToString()
            {
                return "Id:" + Id + ", Name:" + Name + ", Time:" + Time.ToString() + ", Token:" + Token.ToString() + ", Value:" + Value.ToString();
            }
        }

        public static void Run()
        {
            Console.WriteLine("Show Class Demo, the class ClsDemo need StructLayout attr");
            var clsObj = new ClsDemo();
            Console.WriteLine(clsObj);
            var clsBuffer = StructureConverter.ToBytes<ClsDemo>(clsObj);
            var clsResult = StructureConverter.ToStructure<ClsDemo>(clsBuffer);
            Console.WriteLine(clsResult);

            Thread.Sleep(1000);
            
            Console.WriteLine("\nShow Struct Demo");
            var stObj = new StDemo(DateTime.Now);
            Console.WriteLine(stObj);
            var stBuffer = StructureConverter.ToBytes<StDemo>(stObj);
            var stResult = StructureConverter.ToStructure<StDemo>(stBuffer);
            Console.WriteLine(stResult);
        }
    }
}

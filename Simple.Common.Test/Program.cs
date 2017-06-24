using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Common.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Simple.Common.Test._Tests.InstanceCreateTest.Run();
            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }
    }
}

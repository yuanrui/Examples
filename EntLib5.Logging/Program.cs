using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace EntLib5.Logging
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Write("Test wirte Log" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "General");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}

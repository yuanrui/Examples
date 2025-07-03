using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Simple.Data.MySql
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //SequenceTest.Run();
                BulkInsertTest.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("...");
            Console.ReadLine();
            Console.WriteLine("Exit");
        }
    }
}
